using AutoMapper;
using Dto;
using Interface.IRepositories;
using Interface.Model;
using Interface.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Service.QueriesService
{
    public class OrderQuery : IOrderQurey
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderQuery(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<OrderModel> FindAll()
        {
            var model = _unitOfWork.OrderRepository.FindAll().SingleOrDefault();
            var order = _mapper.Map<List<OrderModel>>(model);
            return order;
        }

        public IEnumerable<OrderModel> FindByCondition(Expression<Func<Order, bool>> predicate)
        {
            var model = _unitOfWork.OrderRepository.FindByCondition(predicate).SingleOrDefault();
            var order = _mapper.Map<List<OrderModel>>(model);
            return order;
        }

        public OrderModel Get(int id)
        {
            var model = _unitOfWork.OrderRepository.FindByCondition(o=> o.Id ==id).SingleOrDefault();
            var order = _mapper.Map<OrderModel>(model);
            return order;

        }

        public IEnumerable<OrderModel> GetOrdersByUser(int customerid)
        {
            var model = _unitOfWork.OrderRepository.FindByCondition(o => o.CustomerId == customerid).ToList();
            var order = _mapper.Map<List<OrderModel>>(model);
            return order;
        }
    }
}
