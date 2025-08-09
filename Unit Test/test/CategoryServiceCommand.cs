using AutoMapper;
using Dto;
using Interface.IRepositories;
using Interface.Model;
using Moq;
using Repositories.Repositories;
using Service.CommandService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Unit_test;
using XUnitTest;

namespace Unit_Test.test
{
    public class CategoryServiceCommand : IClassFixture<UnitOfWorkFixture>
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<IMapper> _mapper = new();

        private readonly Mock<ICategoryRepository> _categoryRepo = new();
        private readonly Mock<IProductCategoryRepository> _productCategoryRepo = new();

        public CategoryServiceCommand()
        {
            _uow.SetupGet(x => x.CategoryRepository).Returns(_categoryRepo.Object);
            _uow.SetupGet(x => x.ProductCategoryRepository).Returns(_productCategoryRepo.Object);

            // თუ SaveChanges არის void -> უბრალოდ Setup, თორემ Remove Returns
            _uow.Setup(x => x.SaveChanges());
        }

        // ---------------- CategoryCommand ----------------

        [Fact]
        public void CategoryCommand_Insert_Should_Map_And_Save_And_Return_Id()
        {
            // Arrange
            var model = new CategoryModel { Id = 0, Name = "Phones", Description="iphone" };
            var entity = new Category { Id = 123, Name = "Phones", Description="iphone" };

            _mapper.Setup(m => m.Map<Category>(model)).Returns(entity);
            _categoryRepo.Setup(r => r.Insert(It.IsAny<Category>()))
                         .Callback<Category>(c => c.Id = entity.Id);

            var sut = new CategoryCommand(_uow.Object, _mapper.Object);

            // Act
            var newId = sut.Insert(model);

            // Assert
            Assert.Equal(123, newId);
            _categoryRepo.Verify(r => r.Insert(It.Is<Category>(c => c.Name == "Phones")), Times.Once);
            _uow.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CategoryCommand_Update_Should_Update_When_Entity_Exists()
        {
            // Arrange
            var db = new Category { Id = 10, Name = "Old", Description="tv" };
            var incoming = new CategoryModel { Id = 10, Name = "New",Description="lepton"};
            var mapped = new Category { Id = 0, Name = "New", Description = "lepton" }; // შემდეგ updatcategory.Id = db.Id

            _categoryRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                         .Returns(new[] { db }.AsQueryable());

            _mapper.Setup(m => m.Map<Category>(incoming)).Returns(mapped);

            var sut = new CategoryCommand(_uow.Object, _mapper.Object);

            // Act
            sut.Update(10, incoming);

            // Assert
            _categoryRepo.Verify(r => r.Update(It.Is<Category>(c => c.Id == 10 && c.Name == "New")), Times.Once);
            _uow.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CategoryCommand_Update_Should_DoNothing_When_NotFound()
        {
            _categoryRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                         .Returns(Enumerable.Empty<Category>().AsQueryable());

            var sut = new CategoryCommand(_uow.Object, _mapper.Object);

            sut.Update(99, new CategoryModel { Name = "X", Description = "lepton" });

            _categoryRepo.Verify(r => r.Update(It.IsAny<Category>()), Times.Never);
            _uow.Verify(x => x.SaveChanges(), Times.Never);
        }

        [Fact]
        public void CategoryCommand_Delete_Should_Delete_When_Found()
        {
            // NOTE: შენს კოდში predicate არის c => c.Id == c.Id (ბაგი).
            // აქ დავაყენოთ ისე, რომ repository-მ მაინც დაგვიბრუნოს ერთი ჩანაწერი და წაშლის Verify გამოვიდეს.
            var db = new Category { Id = 5, Name = "ToDelete", Description = "lepton" };

            _categoryRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                         .Returns(new[] { db }.AsQueryable());

            var sut = new CategoryCommand(_uow.Object, _mapper.Object);

            sut.Delete(5);

            _categoryRepo.Verify(r => r.Delete(It.Is<Category>(c => c.Id == 5)), Times.Once);
            _uow.Verify(x => x.SaveChanges(), Times.Once);
        }

        [Fact]
        public void CategoryCommand_Delete_Should_Expose_Bug_When_Many_Match()
        {
            // რადგან predicate არის c => c.Id == c.Id, თუ 2+ ჩანაწერია, SingleOrDefault გამოისვრის
            var list = new List<Category>
        {
            new Category { Id = 1, Name = "A",Description="c" },
            new Category { Id = 2, Name = "B",Description="d" }
        };

            _categoryRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                         .Returns(list.AsQueryable());

            var sut = new CategoryCommand(_uow.Object, _mapper.Object);

            Assert.Throws<InvalidOperationException>(() => sut.Delete(2));
            // 👉 ფიქსი სერვისში:  c => c.Id == id
        }
    }
}