using AutoMapper;
using Dto;
using Interface.Command;
using Interface.IRepositories;
using Interface.Model;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ShippingService> _logger;

        public ShippingService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ShippingService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves a shipping option by its ID.
        /// </summary>
        public ShippingModel GetShippingById(int id)
        {
            var entity = _unitOfWork.ShippingRepository
                .FindByCondition(s => s.Id == id)
                .SingleOrDefault();
            return entity == null ? null : _mapper.Map<ShippingModel>(entity);
        }

        /// <summary>
        /// Retrieves all shipping options.
        /// </summary>
        public IEnumerable<ShippingModel> GetAllShippings()
        {
            var entities = _unitOfWork.ShippingRepository.GetAll();
            return _mapper.Map<IEnumerable<ShippingModel>>(entities);
        }

        /// <summary>
        /// Creates a new shipping option and returns its ID.
        /// </summary>
        public int CreateShipping(ShippingModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var entity = _mapper.Map<Shipping>(model);
            _unitOfWork.ShippingRepository.Insert(entity);
            _unitOfWork.SaveChanges();
            _logger.LogInformation("Created new shipping (ID: {Id})", entity.Id);
            return entity.Id;
        }

        /// <summary>
        /// Updates an existing shipping option.
        /// </summary>
        public bool UpdateShipping(ShippingModel model)
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            var entity = _unitOfWork.ShippingRepository
                .FindByCondition(s => s.Id == model.Id)
                .SingleOrDefault();
            if (entity == null) return false;

            // Map updated fields onto the existing entity
            _mapper.Map(model, entity);
            _unitOfWork.ShippingRepository.Update(entity);
            _unitOfWork.SaveChanges();
            _logger.LogInformation("Updated shipping (ID: {Id})", entity.Id);
            return true;
        }

        /// <summary>
        /// Deletes a shipping option by its ID.
        /// </summary>
        public bool DeleteShipping(int id)
        {
            var entity = _unitOfWork.ShippingRepository
                .FindByCondition(s => s.Id == id)
                .SingleOrDefault();
            if (entity == null) return false;

            _unitOfWork.ShippingRepository.Delete(entity);
            _unitOfWork.SaveChanges();
            _logger.LogInformation("Deleted shipping (ID: {Id})", id);
            return true;
        }
    }
}