using AutoMapper;
using Dto;
using Interface.Command;
using Interface.IRepositories;
using Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            throw new NotImplementedException();
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
            if(order != null)
            {
                order = _mapper.Map<Order>(entityModel);
                _unitOfWork.OrderRepository.Update(order);
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
            public int Checkout(CheckoutModel checkoutModel)
            {
                // 1. Retrieve the user's cart and items
                var cart = _unitOfWork.CartRepository.FindByCondition(c => c.CustomerId == checkoutModel.Customerid).SingleOrDefault();
                if (cart == null || !cart.Items.Any())
                {
                    throw new InvalidOperationException("Cart is empty or does not exist.");
                }

                // 2. Validate stock and calculate total amount
                decimal totalAmount = 0;
                foreach (var cartItem in cart.Items)
                {
                    var product = _unitOfWork.ProductRepository.FindByCondition(p => p.Id == cartItem.ProductId).SingleOrDefault();
                    if (product == null || product.Stock < cartItem.Quantity)
                    {
                        throw new InvalidOperationException($"Insufficient stock for product: {product?.Name ?? "Unknown"}");
                    }

                    totalAmount += cartItem.Quantity * product.Price;
                }

                // 3. Create the Order
                var order = new Order
                {
                    CustomerId = checkoutModel.Customerid,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = totalAmount,
                    Status = OrderStatus.Pending,
                    ShippingAddress = checkoutModel.ShippingAddress,
                    BillingAddress = checkoutModel.BillingAddress,
                    PaymentMethod = checkoutModel.PaymentMethod
                };
                _unitOfWork.OrderRepository.Insert(order);
                _unitOfWork.SaveChanges();

                // 4. Create Order Details and update product stock
                foreach (var cartItem in cart.Items)
                {
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price
                    };
                    _unitOfWork.OrderDetailRepository.Insert(orderDetail);

                    // Update product stock
                    var product = _unitOfWork.ProductRepository.FindByCondition(p => p.Id == cartItem.ProductId).SingleOrDefault();
                    if (product != null)
                    {
                        product.Stock -= cartItem.Quantity;
                        _unitOfWork.ProductRepository.Update(product);
                    }
                }

                // 5. Clear the cart
                foreach (var cartItem in cart.Items.ToList())
                {
                    _unitOfWork.CartItemRepository.Delete(cartItem);
                }
                _unitOfWork.SaveChanges();

                // Return the newly created order ID
                return order.Id;
            }

        }
    }

