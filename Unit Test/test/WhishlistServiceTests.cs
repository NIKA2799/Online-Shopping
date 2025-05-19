using AutoMapper;
using Dto;
using Interface.IRepositories;
using Interface.Model;
using Moq;
using Service.CommandService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.test
{
    public class WhishlistServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IWishlistRepositorty> _wishlistRepoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly WhishlistService _service;

        public WhishlistServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _wishlistRepoMock = new Mock<IWishlistRepositorty>();
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock.Setup(u => u.WishlistRepositorty).Returns(_wishlistRepoMock.Object);
            _service = new WhishlistService(_unitOfWorkMock.Object, _mapperMock.Object);
        }

        [Fact]
        public void GetWishlistByCustomerId_ReturnsWishlist()
        {
            var wishlist = new Wishlist { CustomerId = 1, Items = new List<WishlistItem>() };
            _wishlistRepoMock.Setup(r => r.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Wishlist, bool>>>()))
                .Returns(new List<Wishlist> { wishlist }.AsQueryable());

            var result = _service.GetWishlistByCustomerId(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.CustomerId);
        }

        [Fact]
        public void AddWishlist_AddsItemToWishlist()
        {
            var wishlist = new Wishlist { CustomerId = 1, Items = new List<WishlistItem>() };
            var item = new WishlistItem { Id = 2, ProductId = 3 };
            _wishlistRepoMock.Setup(r => r.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Wishlist, bool>>>()))
                .Returns(new List<Wishlist> { wishlist }.AsQueryable());

            _service.AddWishlist(1, item);

            Assert.Contains(item, wishlist.Items);
            _wishlistRepoMock.Verify(r => r.Update(wishlist), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void RemoveFromWishlist_RemovesItem()
        {
            var item = new WishlistItem { Id = 2, ProductId = 3 };
            var wishlist = new Wishlist { CustomerId = 1, Items = new List<WishlistItem> { item } };
            _wishlistRepoMock.Setup(r => r.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Wishlist, bool>>>()))
                .Returns(new List<Wishlist> { wishlist }.AsQueryable());

            _service.RemoveFromWishlist(1, 2);

            Assert.DoesNotContain(item, wishlist.Items);
            _wishlistRepoMock.Verify(r => r.Update(wishlist), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void ClearWishlist_RemovesAllItems()
        {
            var wishlist = new Wishlist { CustomerId = 1, Items = new List<WishlistItem> { new WishlistItem(), new WishlistItem() } };
            _wishlistRepoMock.Setup(r => r.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Wishlist, bool>>>()))
                .Returns(new List<Wishlist> { wishlist }.AsQueryable());

            _service.ClearWishlist(1);

            Assert.Empty(wishlist.Items);
            _wishlistRepoMock.Verify(r => r.Update(wishlist), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Get_ReturnsWishlistModel()
        {
            var wishlist = new Wishlist { CustomerId = 1, Items = new List<WishlistItem>() };
            var wishlistModel = new WishlistModel { CustomerId = 1, Items = new List<WishlistItem>() };
            _wishlistRepoMock.Setup(r => r.FindByCondition(It.IsAny<System.Linq.Expressions.Expression<System.Func<Wishlist, bool>>>()))
                .Returns(new List<Wishlist> { wishlist }.AsQueryable());
            _mapperMock.Setup(m => m.Map<WishlistModel>(wishlist)).Returns(wishlistModel);

            var result = _service.get(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.CustomerId);
        }
    }
}