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
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public OrderQuery(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public IEnumerable<OrderModel> FindAll()
        {
            // get all entity instances, then map
            var entities = _uow.OrderRepository
                               .FindAll()
                               .ToList();

            return _mapper.Map<IEnumerable<OrderModel>>(entities);
        }

        public IEnumerable<OrderModel> FindByCondition(Expression<Func<Order, bool>> predicate)
        {
            var entities = _uow.OrderRepository
                               .FindByCondition(predicate)
                               .ToList();

            return _mapper.Map<IEnumerable<OrderModel>>(entities);
        }

        public OrderModel Get(int id)
        {
            var entity = _uow.OrderRepository
                             .FindByCondition(o => o.Id == id)
                             .SingleOrDefault();

            return _mapper.Map<OrderModel>(entity);
        }

        public IEnumerable<OrderModel> GetOrdersByUser(int customerId)
        {
            var entities = _uow.OrderRepository
                               .FindByCondition(o => o.CustomerId == customerId)
                               .ToList();

            return _mapper.Map<IEnumerable<OrderModel>>(entities);
        }
    }
}