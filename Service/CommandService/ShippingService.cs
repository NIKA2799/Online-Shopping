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
    public class ShippingService : IShippingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ShippingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // 1. Get Shipping by ID
        public ShippingModel GetShippingById(int id)
        {
            var shipping = _unitOfWork.ShippingRepository.FindByCondition(s => s.Id == id).SingleOrDefault();
            return _mapper.Map<ShippingModel>(shipping);
        }

        // 2. Get All Shippings
        public IEnumerable<ShippingModel> GetAllShippings()
        {
            var shippings = _unitOfWork.ShippingRepository.GetAll().ToList();
            return _mapper.Map<IEnumerable<ShippingModel>>(shippings);
        }

        // 3. Create Shipping
        public int CreateShipping(ShippingModel shippingModel)
        {
            var shippingEntity = _mapper.Map<Shipping>(shippingModel);
            _unitOfWork.ShippingRepository.Insert(shippingEntity);
            _unitOfWork.SaveChanges();
            return shippingEntity.Id;
        }

        // 4. Update Shipping
        public bool UpdateShipping(ShippingModel shippingModel)
        {
            var existingShipping = _unitOfWork.ShippingRepository.FindByCondition(s => s.Id == shippingModel.Id).SingleOrDefault();
            if (existingShipping == null) return false;

            var updateshipping = _mapper.Map<Shipping>(shippingModel);
            updateshipping.Id = existingShipping.Id;
            _unitOfWork.SaveChanges();
            return true;
        }

        // 5. Delete Shipping
        public bool DeleteShipping(int id)
        {
            var shipping = _unitOfWork.ShippingRepository.FindByCondition(s => s.Id == id).SingleOrDefault();
            if (shipping == null) return false;

            _unitOfWork.ShippingRepository.Delete(shipping);
            _unitOfWork.SaveChanges();
            return true;
        }
    }
}