using AutoMapper;
using Dto;
using Interface.IRepositories;
using Moq;
using Service.CommandService;
using System.Linq.Expressions;
using Webdemo.Models;

namespace Unit_Test.test
{
    public class ReviewCommandServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IReviewRepository> _reviewRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ReviewCommandService _service;

        public ReviewCommandServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _reviewRepoMock = new Mock<IReviewRepository>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock.Setup(u => u.ReviewRepository).Returns(_reviewRepoMock.Object);
            _service = new ReviewCommandService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void Delete_RemovesReview_WhenExists()
        {
            var review = new Review { Id = 1 };
           _reviewRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
                .Returns(new[] { review }.AsQueryable());
            _service.Delete(1);
            _reviewRepoMock.Verify(r => r.Delete(review), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Delete_DoesNothing_WhenNotExists()
        {
            _reviewRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
                .Returns(Enumerable.Empty<Review>().AsQueryable());
            _service.Delete(1);
            _reviewRepoMock.Verify(r => r.Delete(It.IsAny<Review>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Never);
        }

        [Fact]
        public void Insert_Throws_WhenReviewExists()
        {
            var model = new ReviewModel { ProductId = 1, CustomerId = 2 };
            _reviewRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
                .Returns(new[] { new Review() }.AsQueryable());
            Assert.Throws<InvalidOperationException>(() => _service.Insert(model));
        }

        [Fact]
        public void Insert_AddsReview_WhenNotExists()
        {
            var model = new ReviewModel { ProductId = 1, CustomerId = 2 };
            var review = new Review { Id = 10 };
            _reviewRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
             .Returns(Enumerable.Empty<Review>().AsQueryable());
            _mapperMock.Setup(m => m.Map<Review>(model)).Returns(review);
            var result = _service.Insert(model);
            _reviewRepoMock.Verify(r => r.Insert(review), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
            Assert.Equal(10, result);
        }

        [Fact]
        public void Update_UpdatesReview_WhenExists()
        {
            var model = new ReviewModel { ProductId = 1, CustomerId = 2 };
            var existing = new Review { Id = 5 };
            var mapped = new Review { Id = 5 };
            _reviewRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
                .Returns(new[] { existing }.AsQueryable());
            _mapperMock.Setup(m => m.Map<Review>(model)).Returns(mapped);
            _service.Update(5, model);
            _reviewRepoMock.Verify(r => r.Update(existing), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Update_DoesNothing_WhenNotExists()
        {
            var model = new ReviewModel { ProductId = 1, CustomerId = 2 };
            _reviewRepoMock.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Review, bool>>>()))
                 .Returns(Enumerable.Empty<Review>().AsQueryable());
            _service.Update(5, model);
            _reviewRepoMock.Verify(r => r.Update(It.IsAny<Review>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Never);
        }
    }
}


