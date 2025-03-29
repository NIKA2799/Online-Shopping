using AutoMapper;
using Dto;
using Interface.IRepositories;
using Interface.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;

namespace Service.QueriesService
{
    public class ReviewQureyService : IReviewQureyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReviewQureyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public IEnumerable<ReviewModel> FindAll()
        {
            var model = _unitOfWork.ReviewRepository.GetAll();
            var review = _mapper.Map<List<ReviewModel>>(model);
            return review;
        }

        public IEnumerable<ReviewModel> FindByCondition(Expression<Func<Review, bool>> predicate)
        {
            var model = _unitOfWork.ReviewRepository.FindByCondition(predicate);
            var review = _mapper.Map<List<ReviewModel>>(model);
            return review;
        }

        public ReviewModel Get(int id)
        {
            var model = _unitOfWork.ReviewRepository.FindByCondition(r=> r.Id==id).SingleOrDefault();
            var review = _mapper.Map<ReviewModel>(model);
            return review;
        }
    }
}
