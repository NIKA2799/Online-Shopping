using AutoMapper;
using Dto;
using Interface.IRepositories;
using Interface.Model;
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
    public class CategoryQureyTest
    {
        private readonly Mock<IUnitOfWork> _uow = new();
        private readonly Mock<IMapper> _mapper = new();

        private readonly Mock<ICategoryRepository> _categoryRepo = new();
        private readonly Mock<IProductCategoryRepository> _productCategoryRepo = new();

        public CategoryQureyTest()
        {
            _uow.SetupGet(x => x.CategoryRepository).Returns(_categoryRepo.Object);
            _uow.SetupGet(x => x.ProductCategoryRepository).Returns(_productCategoryRepo.Object);

            // თუ SaveChanges არის void -> უბრალოდ Setup, თორემ Remove Returns
            _uow.Setup(x => x.SaveChanges());
        }
        [Fact]
        public void CategoryQurey_FindAll_Should_Map_List()
        {
            var data = new List<Category>
        {
            new Category { Id = 1, Name = "Phones", Description="samsung"},
            new Category { Id = 2, Name = "Laptops", Description="ergerg" }
        };

            _categoryRepo.Setup(r => r.FindAll()).Returns(data.AsQueryable());

            _mapper.Setup(m => m.Map<List<CategoryModel>>(data))
                   .Returns(new List<CategoryModel>
                   {
                   new CategoryModel{ Id = 1, Name = "Phones",Description="samsung"},
                   new CategoryModel{ Id = 2, Name = "Laptops", Description="ergerg"},
                   });

            var sut = new CategoryQurey(_uow.Object, _mapper.Object);

            var result = sut.FindAll().ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Name == "Phones");
            Assert.Contains(result, x => x.Name == "Laptops");
        }

        [Fact]
        public void CategoryQurey_Get_Should_Map_Single_Category()
        {
            var db = new Category { Id = 10, Name = "Audio", Description= "test1" };

            _categoryRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                         .Returns(new[] { db }.AsQueryable());

            _mapper.Setup(m => m.Map<CategoryModel>(db))
                   .Returns(new CategoryModel { Id = 10, Name = "Audio", Description="test1"});

            var sut = new CategoryQurey(_uow.Object, _mapper.Object);

            var model = sut.Get(10);

            Assert.NotNull(model);
            Assert.Equal(10, model.Id);
            Assert.Equal("Audio", model.Name);
        }

        [Fact]
        public void CategoryQurey_FindByCondition_Should_Currently_Fail_Because_Of_Wrong_Map()
        {
            // სერვისში:
            // var model = repo.FindByCondition(predicate).SingleOrDefault();
            // var category = _mapper.Map<List<CategoryModel>>(model); // <- ბაგი
            // აქ ვაიძულებთ Mapper-ს რომ List-ზე mapping-ზე exception იდოს
            _categoryRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<Category, bool>>>()))
                         .Returns(new[] { new Category { Id = 1, Name = "Phones", Description= "test2" } }.AsQueryable());

            _mapper.Setup(m => m.Map<List<CategoryModel>>(It.IsAny<Category>()))
                   .Throws(new AutoMapperMappingException("Cannot map Category to List<CategoryModel>"));

            var sut = new CategoryQurey(_uow.Object, _mapper.Object);

            Assert.Throws<AutoMapperMappingException>(() =>
                sut.FindByCondition(c => c.Name == "Phones").ToList());

            // 👉 ფიქსი სერვისში:
            // var list = _unitOfWork.CategoryRepository.FindByCondition(predicate).ToList();
            // return _mapper.Map<List<CategoryModel>>(list);
        }

        [Fact]
        public void CategoryQurey_GetProductsByCategory_Should_Return_Projected_Products()
        {
            var pcs = new List<ProductCategory>
        {
            new ProductCategory
            {
                CategoryId = 7,
                Product = new Product { Id = 100, Name = "iPhone", Price = 999, Description="2025", ImageUrl="http://example.com/image.jpg", Stock=50 }
            },
            new ProductCategory
            {
                CategoryId = 7,
                Product = new Product { Id = 200, Name = "iPad", Price = 799, Description="modern", ImageUrl="http://example.com/image.jpg", Stock=50 }
            }
        };

            _productCategoryRepo.Setup(r => r.FindByCondition(It.IsAny<Expression<Func<ProductCategory, bool>>>()))
                                .Returns(pcs.AsQueryable());

            _mapper.Setup(m => m.Map<IEnumerable<ProductModel>>(It.IsAny<IEnumerable<Product>>()))
                   .Returns<IEnumerable<Product>>(pp => pp.Select(p => new ProductModel
                   {
                       Id = p.Id,
                       Name = p.Name,
                       Price = p.Price,
                       Description=p.Description,
                       Stock=p.Stock,
                       Items=p.Items,
                       IsFeatured=p.IsFeatured,
                       ImageUrl=p.ImageUrl,
                       ImagePath=p.ImagePath
                       

                   }));

            var sut = new CategoryQurey(_uow.Object, _mapper.Object);

            var result = sut.GetProductsByCategory(7).ToList();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Name == "iPhone");
            Assert.Contains(result, p => p.Name == "iPad");
        }
    }
}