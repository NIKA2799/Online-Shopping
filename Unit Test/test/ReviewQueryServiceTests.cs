using AutoMapper;
using Dto;
using Interface.IRepositories;
using Moq;
using Service.QueriesService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;

namespace Unit_Test.test
{
    public class ReviewQueryServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ReviewQueryService _reviewService;

        public ReviewQueryServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _reviewService = new ReviewQueryService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetReviewsByProduct_ShouldReturnMappedReviews()
        {
            // Arrange
            var productId = 1;
            var reviews = new List<Review> { new Review { Id = 1, ProductId = productId } };
            var mappedReviews = new List<ReviewModel> { new ReviewModel { Id = 1 } };

            _unitOfWorkMock.Setup(u => u.ReviewRepository.FindByCondition(r => r.ProductId == productId))
                .Returns(reviews.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<ReviewModel>>(reviews)).Returns(mappedReviews);

            // Act
            var result = _reviewService.GetReviewsByProduct(productId);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public void GetReviewByUser_ShouldReturnMappedReview()
        {
            // Arrange
            int productId = 1, customerId = 2;
            var review = new Review { Id = 1, ProductId = productId, CustomerId = customerId };
            var mappedReview = new ReviewModel { Id = 1 };

            _unitOfWorkMock.Setup(u =>
                u.ReviewRepository.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
                .Returns(new List<Review> { review }.AsQueryable());

            _mapperMock.Setup(m => m.Map<ReviewModel>(review)).Returns(mappedReview);

            // Act
            var result = _reviewService.GetReviewByUser(productId, customerId);

            // Assert
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public void FindByCondition_ShouldReturnMappedList()
        {
            // Arrange
            var reviews = new List<Review> { new Review { Id = 1 }, new Review { Id = 2 } };
            var mapped = new List<ReviewModel> { new ReviewModel { Id = 1 }, new ReviewModel { Id = 2 } };

            _unitOfWorkMock.Setup(u => u.ReviewRepository.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
                .Returns(reviews.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<ReviewModel>>(reviews)).Returns(mapped);

            // Act
            var result = _reviewService.FindByCondition(r => r.Id > 0);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Id == 1);
            Assert.Contains(result, r => r.Id == 2);
        }
    }
}
