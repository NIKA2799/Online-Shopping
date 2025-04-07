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
    public class ReviewQueryService : IReviewQureyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReviewQueryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<ReviewModel> GetReviewsByProduct(int productId)
        {
            var reviews = _unitOfWork.ReviewRepository.FindByCondition(r => r.ProductId == productId).ToList();
            return _mapper.Map<List<ReviewModel>>(reviews);
        }

        
        public ReviewModel GetReviewByUser(int productId, int customerId)
        {
            var review = _unitOfWork.ReviewRepository
                .FindByCondition(r => r.ProductId == productId && r.CustomerId == customerId)
                .SingleOrDefault();

            return _mapper.Map<ReviewModel>(review);
        }

       
        public IEnumerable<ReviewModel> FindByCondition(Expression<Func<Review, bool>> predicate)
        {
            var model = _unitOfWork.ReviewRepository.FindByCondition(predicate);
            return _mapper.Map<List<ReviewModel>>(model);
        }
    }
}