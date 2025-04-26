using AutoMapper;
using Dto;
using Interface.Command;
using Interface.IRepositories;
using Interface.Model;
using Microsoft.EntityFrameworkCore;

namespace Service.CommandService
{
    public class OrderCommand : IOrderCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderCommand(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public void CancelOrder(int orderId)
        {
            var order = _unitOfWork.OrderRepository.FindByCondition(o => o.Id == orderId).SingleOrDefault();
            if (order != null && order.Status != OrderStatus.Cancelled)
            {
                order.Status = OrderStatus.Cancelled;
                _unitOfWork.OrderRepository.Update(order);

               
                foreach (var detail in _unitOfWork.OrderDetailRepository.FindByCondition(d => d.OrderId == orderId))
                {
                    var product = _unitOfWork.ProductRepository.GetById(detail.ProductId);
                    if (product != null)
                    {
                        product.Stock += detail.Quantity;
                        product.IsOutOfStock = product.Stock <= 0;
                        _unitOfWork.ProductRepository.Update(product);
                    }
                }

                _unitOfWork.SaveChanges();
            }
        }
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public int Insert(OrderModel entityModel)
        {
            var orderEntity = _mapper.Map<Order>(entityModel);
            orderEntity.OrderDate = DateTime.UtcNow;
            orderEntity.Status = OrderStatus.Pending;

            _unitOfWork.OrderRepository.Insert(orderEntity);
            _unitOfWork.SaveChanges();

            return orderEntity.Id;
        }

        public void Update(int id, OrderModel entityModel)
        {
            var order = _unitOfWork.OrderRepository.FindByCondition(o => o.Id == id).SingleOrDefault();
            if (order != null)
            {
                var updateorder = _mapper.Map<Order>(entityModel);
                updateorder.Id =order.Id;
                _unitOfWork.OrderRepository.Update(updateorder);
                _unitOfWork.SaveChanges();
            }
        }

        public void UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = _unitOfWork.OrderRepository.FindByCondition(o => o.Id == orderId).SingleOrDefault();
            if (order != null)
            {
                order.Status = status;
                _unitOfWork.OrderRepository.Update(order);
                _unitOfWork.SaveChanges();
            }
        }
        public int Checkout(CheckoutModel model)
        {
            // 1. Load cart + items
            var cart = _unitOfWork.CartRepository
                           .FindByCondition(c => c.CustomerId == model.Customerid)
                           .Include(c => c.Items)
                           .ThenInclude(i => i.Product)
                           .SingleOrDefault();
            if (cart?.Items == null || !cart.Items.Any())
                throw new InvalidOperationException("Cart is empty or does not exist.");

            // 2. Validate stock and compute total
            decimal total = 0;
            foreach (var item in cart.Items)
            {
                if (item.Product.Stock < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for {item.Product.Name}.");
                total += item.Quantity * item.Product.Price;
            }

            // 3. Build order + details
            var order = new Order
            {
                CustomerId = model.Customerid,
                OrderDate = DateTime.UtcNow,
                TotalAmount = total,
                Status = OrderStatus.Pending,
                ShippingAddress = model.ShippingAddress,
                BillingAddress = model.BillingAddress,
                PaymentMethod = model.PaymentMethod
            };
            _unitOfWork.OrderRepository.Insert(order);

            foreach (var item in cart.Items)
            {
                var detail = new OrderDetail
                {
                    Order = order,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.Price
                };
                _unitOfWork.OrderDetailRepository.Insert(detail);

                // adjust stock
                item.Product.Stock -= item.Quantity;
                _unitOfWork.ProductRepository.Update(item.Product);
            }

            // 4. Clear cart
            _unitOfWork.CartItemRepository.DeleteRange(cart.Items);

            // 5. Commit once
            _unitOfWork.SaveChanges();
            return order.Id;
        }
        public OrderStatus? TrackOrderStatus(int orderId, int userId)
        {
            var order = _unitOfWork.OrderRepository
                .FindByCondition(o => o.Id == orderId && o.UserId == userId)
                .SingleOrDefault();

            if (order == null)
                return null;

            return order.Status;
        }
    }
}

