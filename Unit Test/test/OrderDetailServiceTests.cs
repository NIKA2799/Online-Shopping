using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Xunit;

// მოარგე namespaces შენს 솔უშენზე:
using Service.CommandService;
using Interface.IRepositories;
using Interface.Model;     // OrderDetail
using Dto;                 // OrderDetailModel

public class OrderDetailServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<IOrderDetailRepository> _repo = new();

    private readonly OrderDetailService _sut;

    public OrderDetailServiceTests()
    {
        _uow.SetupGet(x => x.OrderDetailRepository).Returns(_repo.Object);

        // თუ SaveChanges() არის void:
        _uow.Setup(x => x.SaveChanges());

        _sut = new OrderDetailService(_uow.Object, _mapper.Object);
    }

    // ---------- Insert ----------
    [Fact]
    public void Insert_Should_Map_Insert_Save_And_Return_NEW_Id() // ეს ტესტი ამხელს ბაგს Insert-ში
    {
        var model = new OrderDetailModel { Id = 0 /* create flow */ };
        var entity = new OrderDetail { Id = 0 };

        _mapper.Setup(m => m.Map<OrderDetail>(model)).Returns(entity);

        // Insert-ისას რეპო ასიგნებს ახალ Id-ს
        _repo.Setup(r => r.Insert(It.IsAny<OrderDetail>()))
             .Callback<OrderDetail>(od => od.Id = 123);

        var newId = _sut.Insert(model);

        // მოველით, რომ დაბრუნდეს DB-დან წამოსული ახალი Id (123),
        // მაგრამ ახლა მეთოდი აბრუნებს model.Id-ს, რადგან წერს order.Id = orderDetail.Id;
        Assert.Equal(123, newId); // ❗ ეს ტესტი ჩავარდება, სანამ Insert-ს არ გაასწორებ
    }

    // ---------- Update ----------
    [Fact]
    public void Update_Should_Update_And_Save_When_Found()
    {
        var db = new OrderDetail { Id = 10 };
        var incoming = new OrderDetailModel { Id = 10 }; // სხვა ველებიც შეიძლება გქონდეს

        _repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
             .Returns(new[] { db }.AsQueryable());

        var mapped = new OrderDetail { Id = 0 }; // სერვისი გადააწერს Id-ს db.Id-ზე
        _mapper.Setup(m => m.Map<OrderDetail>(incoming)).Returns(mapped);

        _sut.Update(incoming, id: 10);

        _repo.Verify(r => r.Update(It.Is<OrderDetail>(od => od.Id == 10)), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Update_Should_DoNothing_When_NotFound()
    {
        _repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
             .Returns(Enumerable.Empty<OrderDetail>().AsQueryable());

        _sut.Update(new OrderDetailModel { Id = 99 }, id: 99);

        _repo.Verify(r => r.Update(It.IsAny<OrderDetail>()), Times.Never);
        _uow.Verify(u => u.SaveChanges(), Times.Never);
    }

    // ---------- Delete ----------
    [Fact]
    public void Delete_Should_Delete_And_Save_When_Found()
    {
        var db = new OrderDetail { Id = 7 };

        _repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
             .Returns(new[] { db }.AsQueryable());

        _sut.Delete(7);

        _repo.Verify(r => r.Delete(It.Is<OrderDetail>(od => od.Id == 7)), Times.Once);
        _uow.Verify(u => u.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Delete_ShouldThrow_KeyNotFound_When_NotFound()
    {
        _repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
             .Returns(Enumerable.Empty<OrderDetail>().AsQueryable());

        Assert.Throws<KeyNotFoundException>(() => _sut.Delete(999));
        _repo.Verify(r => r.Delete(It.IsAny<OrderDetail>()), Times.Never);
        _uow.Verify(u => u.SaveChanges(), Times.Never);
    }

    // ---------- GetAll ----------
    [Fact]
    public void GetAll_Should_Map_List()
    {
        var data = new List<OrderDetail>
        {
            new OrderDetail { Id = 1 },
            new OrderDetail { Id = 2 }
        };

        _repo.Setup(r => r.FindAll()).Returns(data.AsQueryable());

        _mapper.Setup(m => m.Map<List<OrderDetailModel>>(data))
               .Returns(new List<OrderDetailModel>
               {
                   new OrderDetailModel { Id = 1 },
                   new OrderDetailModel { Id = 2 }
               });

        var res = _sut.GetAll().ToList();

        Assert.Equal(2, res.Count);
        Assert.Contains(res, x => x.Id == 1);
        Assert.Contains(res, x => x.Id == 2);
    }

    // ---------- GetById ----------
    [Fact]
    public void GetById_Should_Map_When_Found()
    {
        var db = new OrderDetail { Id = 5 };

        _repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
             .Returns(new[] { db }.AsQueryable());

        _mapper.Setup(m => m.Map<OrderDetailModel>(db)).Returns(new OrderDetailModel { Id = 5 });

        var res = _sut.GetById(5);

        Assert.NotNull(res);
        Assert.Equal(5, res.Id);
    }

    [Fact]
    public void GetById_Should_Return_Null_When_NotFound()
    {
        _repo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<OrderDetail, bool>>>()))
             .Returns(Enumerable.Empty<OrderDetail>().AsQueryable());

        // mapper null-ს დაუბრუნებს
        _mapper.Setup(m => m.Map<OrderDetailModel>(null)).Returns((OrderDetailModel)null);

        var res = _sut.GetById(123);

        Assert.Null(res);
    }
}
