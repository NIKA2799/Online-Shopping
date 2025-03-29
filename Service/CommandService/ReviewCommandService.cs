using AutoMapper;
using Dto;
using Interface.Command;
using Interface.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Webdemo.Models;

namespace Service.CommandService
{
    public class ReviewCommandService : IReviewCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReviewCommandService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public void Delete(int id)
        {
            var reviw = _unitOfWork.ReviewRepository.FindByCondition(u => u.Id == id).SingleOrDefault();
            if(reviw != null)
            {
                _unitOfWork.ReviewRepository.Delete(reviw);
                _unitOfWork.SaveChanges();
            }
        }

        public int Insert(ReviewModel entityModel)
        {
            var reviw = _mapper.Map<Review>(entityModel);
            _unitOfWork.ReviewRepository.Insert(reviw);
            _unitOfWork.SaveChanges();
            reviw.Id = entityModel.Id;
            return reviw.Id;
        }

        public void Update(int id, ReviewModel entityModel)
        {
            var reviw = _unitOfWork.ReviewRepository.FindByCondition(r => r.Id == id).SingleOrDefault();
            if(reviw != null)
            {
                var updatereviw = _mapper.Map<Review>(entityModel);
                updatereviw.Id = reviw.Id;
                _unitOfWork.ReviewRepository.Update(reviw);
                _unitOfWork.SaveChanges();
            }
        }
    }
}
