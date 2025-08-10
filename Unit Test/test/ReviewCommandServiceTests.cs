using AutoMapper;
using Interface.IRepositories;
using Moq;
using Service.CommandService;
using Webdemo.Models;
using Dto;
using Xunit;

public class ReviewCommandServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ReviewCommandService _service;

    public ReviewCommandServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _service = new ReviewCommandService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    // -------- INSERT TESTS --------

    [Fact]
    public void Insert_ShouldThrow_WhenReviewAlreadyExists()
    {
        var model = new ReviewModel { ProductId = 1, CustomerId = 2 };
        _unitOfWorkMock.Setup(u => u.ReviewRepository.FindByCondition(
            It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>()))
            .Returns(new List<Review> { new Review() }.AsQueryable());

        Assert.Throws<InvalidOperationException>(() => _service.Insert(model));
    }

    [Fact]
    public void Insert_ShouldAdd_WhenReviewDoesNotExist()
    {
        var model = new ReviewModel { ProductId = 1, CustomerId = 2 };
        var reviewEntity = new Review { Id = 10 };

        _unitOfWorkMock.Setup(u => u.ReviewRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>()))
            .Returns(new List<Review>().AsQueryable());

        _mapperMock.Setup(m => m.Map<Review>(model)).Returns(reviewEntity);

        var result = _service.Insert(model);

        Assert.Equal(10, result);
        _unitOfWorkMock.Verify(u => u.ReviewRepository.Insert(reviewEntity), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Insert_ShouldThrow_WhenModelIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _service.Insert(null));
    }

    // -------- DELETE TESTS --------

    [Fact]
    public void Delete_ShouldRemove_WhenReviewExists()
    {
        var review = new Review { Id = 1 };
        _unitOfWorkMock.Setup(u => u.ReviewRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>()))
            .Returns(new List<Review> { review }.AsQueryable());

        _service.Delete(1);

        _unitOfWorkMock.Verify(u => u.ReviewRepository.Delete(review), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_ShouldDoNothing_WhenReviewDoesNotExist()
    {
        _unitOfWorkMock.Setup(u => u.ReviewRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>()))
            .Returns(new List<Review>().AsQueryable());

        _service.Delete(1);

        _unitOfWorkMock.Verify(u => u.ReviewRepository.Delete(It.IsAny<Review>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Never);
    }

    [Fact]
    public void Delete_ShouldThrow_WhenIdIsInvalid()
    {
        Assert.Throws<ArgumentException>(() => _service.Delete(0));
    }

    // -------- UPDATE TESTS --------

    [Fact]
    public void Update_ShouldModify_WhenReviewExists()
    {
        var existingReview = new Review { Id = 1 };
        var model = new ReviewModel { ProductId = 1, CustomerId = 2 };
        var mapped = new Review { Id = 1 };

        _unitOfWorkMock.Setup(u => u.ReviewRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>()))
            .Returns(new List<Review> { existingReview }.AsQueryable());

        _mapperMock.Setup(m => m.Map<Review>(model)).Returns(mapped);

        _service.Update(1, model);

        _unitOfWorkMock.Verify(u => u.ReviewRepository.Update(existingReview), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_ShouldDoNothing_WhenReviewDoesNotExist()
    {
        var model = new ReviewModel { ProductId = 1, CustomerId = 2 };

        _unitOfWorkMock.Setup(u => u.ReviewRepository.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<Func<Review, bool>>>()))
            .Returns(new List<Review>().AsQueryable());

        _service.Update(1, model);

        _unitOfWorkMock.Verify(u => u.ReviewRepository.Update(It.IsAny<Review>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Never);
    }

    [Fact]
    public void Update_ShouldThrow_WhenModelIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _service.Update(1, null));
    }

    [Fact]
    public void Update_ShouldThrow_WhenIdIsInvalid()
    {
        var model = new ReviewModel { ProductId = 1, CustomerId = 2 };
        Assert.Throws<ArgumentException>(() => _service.Update(0, model));
    }
}
