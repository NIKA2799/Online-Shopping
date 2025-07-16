using AutoMapper;
using Dto;
using Interface;
using Interface.Command;
using Interface.IRepositories;
using Interface.Model;
using Microsoft.EntityFrameworkCore;

namespace Service.CommandService
{
    public class OrderCommandService : IOrderCommandService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IDiscountService _discountService;
        private readonly IAuditService _audit;

        public OrderCommandService(
            IUnitOfWork uow,
            IMapper mapper,
            IDiscountService discountService,
            IAuditService audit)
        {
            _uow = uow;
            _mapper = mapper;
            _discountService = discountService;
            _audit = audit;
        }

        public int Insert(OrderModel model)
        {
            var order = _mapper.Map<Order>(model);
            order.OrderDate = DateTime.UtcNow;
            order.Status = OrderStatus.Pending;

            _uow.OrderRepository.Insert(order);
            _uow.SaveChanges();

            _audit.Log("System", nameof(Order), order.Id.ToString(), "Order created.");
            return order.Id;
        }

        public bool Update(int id, OrderModel model)
        {
            var existing = _uow.OrderRepository
                               .FindByCondition(o => o.Id == id)
                               .SingleOrDefault();
            if (existing == null) return false;

            var updateorder = _mapper.Map<Order>(model);
            updateorder.Id = existing.Id;
            _uow.OrderRepository.Update(updateorder);
            _uow.SaveChanges();

            _audit.Log("System", nameof(Order), id.ToString(), "Updated order");
            return true;
        }

        public bool Delete(int id)
        {
            var order = _uow.OrderRepository
                            .FindByCondition(o => o.Id == id)
                            .SingleOrDefault();
            if (order == null) return false;

            _uow.OrderRepository.Delete(order);
            _uow.SaveChanges();

            _audit.Log("System", nameof(Order), id.ToString(), "Order deleted.");
            return true;
        }

        public bool UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = _uow.OrderRepository
                            .FindByCondition(o => o.Id == orderId)
                            .SingleOrDefault();
            if (order == null) return false;

            order.Status = newStatus;
            _uow.OrderRepository.Update(order);
            _uow.SaveChanges();

            _audit.Log("System", nameof(Order), orderId.ToString(), $"Status changed to {newStatus}.");
            return true;
        }

        public bool CancelOrder(int orderId, int customerId)
        {
            var order = _uow.OrderRepository
                            .FindByCondition(o => o.Id == orderId && o.CustomerId == customerId)
                            .SingleOrDefault();
            if (order == null || order.Status == OrderStatus.Cancelled)
                return false;

            _uow.BeginTransaction();
            try
            {
                var details = _uow.OrderDetailRepository
                                  .FindByCondition(d => d.OrderId == orderId)
                                  .ToList();
                foreach (var d in details)
                {
                    var prod = _uow.ProductRepository.GetById(d.ProductId);
                    if (prod != null)
                    {
                        prod.Stock += d.Quantity;
                        _uow.ProductRepository.Update(prod);
                    }
                }

                order.Status = OrderStatus.Cancelled;
                _uow.OrderRepository.Update(order);

                _uow.SaveChanges();
                _uow.Commit();

                _audit.Log("System", nameof(Order), orderId.ToString(), $"Cancelled by customer {customerId}.");
                return true;
            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }

        public int Checkout(CheckoutModel model)
        {
            var cart = _uow.CartRepository
                           .FindByCondition(c => c.CustomerId == model.Customerid)
                           .Include(c => c.Items)
                           .ThenInclude(i => i.Product)
                           .SingleOrDefault()
                       ?? throw new InvalidOperationException("Cart not found.");

            if (!cart.Items.Any())
                throw new InvalidOperationException("Cart is empty.");

            decimal total = 0m;
            foreach (var item in cart.Items)
            {
                if (item.Product.Stock < item.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for '{item.Product.Name}'.");
                total += item.Quantity * item.Product.Price;
            }

            if (!string.IsNullOrWhiteSpace(model.DiscountCode) &&
                _discountService.IsValid(model.DiscountCode))
            {
                total = _discountService.ApplyDiscount(model.DiscountCode, total);
                _audit.Log("System", nameof(Order), "N/A",
                    $"Applied discount '{model.DiscountCode}' to order (new total: {total:C}).");
            }

            var order = new Order
            {
                CustomerId = model.Customerid,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = total,
                ShippingAddress = model.ShippingAddress,
                BillingAddress = model.BillingAddress,
                PaymentMethod = model.PaymentMethod
            };
            _uow.OrderRepository.Insert(order);

            foreach (var item in cart.Items)
            {
                _uow.OrderDetailRepository.Insert(new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                });

                item.Product.Stock -= item.Quantity;
                _uow.ProductRepository.Update(item.Product);
            }

            _uow.CartItemRepository.DeleteRange(cart.Items);

            _uow.SaveChanges();
            _audit.Log("System", nameof(Order), order.Id.ToString(),
                $"Checked out by customer {model.Customerid}.");

            return order.Id;
        }

        public OrderStatus? TrackOrderStatus(int orderId, int customerId)
        {
            var order = _uow.OrderRepository
                            .FindByCondition(o => o.Id == orderId && o.CustomerId == customerId)
                            .SingleOrDefault();
            return order?.Status;
        }
    }
}