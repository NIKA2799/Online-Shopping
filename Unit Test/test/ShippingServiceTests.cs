namespace Unit_Test.test
{
    using Xunit;
    using Moq;
    using AutoMapper;
    using Service.CommandService;
    using Interface.IRepositories;
    using Interface.Model;
    using Dto;
    using System.Collections.Generic;
    using System.Linq;

    public class ShippingServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IShippingRepository> _shippingRepoMock;
        private readonly IMapper _mapper;
        private readonly ShippingService _service;

        public ShippingServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _shippingRepoMock = new Mock<IShippingRepository>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Shipping, ShippingModel>().ReverseMap();
            });
            _mapper = config.CreateMapper();

            _unitOfWorkMock.Setup(u => u.ShippingRepository).Returns(_shippingRepoMock.Object);

            _service = new ShippingService(_unitOfWorkMock.Object, _mapper);
        }

        [Fact]
        public void GetShippingById_ReturnsShippingModel_WhenFound()
        {
            // Arrange
            var shipping = new Shipping { Id = 1, ShippingAddress = "123 Main St" };
            _shippingRepoMock.Setup(r => r.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Shipping, bool>>>()))
                .Returns(new List<Shipping> { shipping }.AsQueryable());

            // Act
            var result = _service.GetShippingById(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("123 Main St", result.ShippingAddress);
        }
    }
}
