using AutoMapper;
using Dto;
using Interface.IRepositories;
using Interface.Model;

namespace Service.CommandService
{
    public class OrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public int Insert(OrderDetailModel orderDetail)
        {
            var order = _mapper.Map<OrderDetail>(orderDetail);
            _unitOfWork.OrderDetailRepository.Insert(order);
            _unitOfWork.SaveChanges();
            order.Id = orderDetail.Id;
            return order.Id;
        }
        public void Update(OrderDetailModel orderDetailModel, int id)
        {
            var order = _unitOfWork.OrderDetailRepository.FindByCondition(o => o.Id == id).SingleOrDefault();
            if (order != null)
            {
                var updatorder = _mapper.Map<OrderDetail>(orderDetailModel);
                updatorder.Id = order.Id;
                _unitOfWork.OrderDetailRepository.Update(updatorder);
                _unitOfWork.SaveChanges();
            }
        }
        public void Delete(int id)
        {
            var order = _unitOfWork.OrderDetailRepository.FindByCondition(o => o.Id == id).SingleOrDefault();
            if (order == null)
            {
                // Handle the case where the orderDetail was not found
                throw new KeyNotFoundException("OrderDetail not found");
            }
            else
            {
                _unitOfWork.OrderDetailRepository.Delete(order);
                _unitOfWork.SaveChanges();
            }
        }
        public IEnumerable<OrderDetailModel> GetAll()
        {
            var model = _unitOfWork.OrderDetailRepository.FindAll();
            var order = _mapper.Map<List<OrderDetailModel>>(model);
            return order;
        }
        public OrderDetailModel GetById(int id)
        {
            var model = _unitOfWork.OrderDetailRepository.FindByCondition(o => o.Id == id).SingleOrDefault();
            var order = _mapper.Map<OrderDetailModel>(model);
            return order;

        }
    }
}
