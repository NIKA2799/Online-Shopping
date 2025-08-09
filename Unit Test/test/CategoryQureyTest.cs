using AutoMapper;
using Interface.IRepositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
