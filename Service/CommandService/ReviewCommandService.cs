using AutoMapper;
using Dto;
using Interface.Command;
using Interface.IRepositories;
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
            if (reviw != null)
            {
                _unitOfWork.ReviewRepository.Delete(reviw);
                _unitOfWork.SaveChanges();
            }
        }

        public int Insert(ReviewModel entityModel)
        {
            var alreadyExists = _unitOfWork.ReviewRepository
                .FindByCondition(r => r.ProductId == entityModel.ProductId && r.CustomerId == entityModel.CustomerId)
                .Any();

            if (alreadyExists)
                throw new InvalidOperationException("ამ პროდუქტზე უკვე გიწერია შეფასება.");

            var review = _mapper.Map<Review>(entityModel);
            review.DatePosted = DateTime.UtcNow;
            _unitOfWork.ReviewRepository.Insert(review);
            _unitOfWork.SaveChanges();
            return review.Id;
        }


        public void Update(int id, ReviewModel entityModel)
        {
            var reviw = _unitOfWork.ReviewRepository.FindByCondition(r => r.Id == id).SingleOrDefault();
            if (reviw != null)
            {
                var updatereviw = _mapper.Map<Review>(entityModel);
                updatereviw.Id = reviw.Id;
                _unitOfWork.ReviewRepository.Update(reviw);
                _unitOfWork.SaveChanges();
            }
        }
    }
}
