using Dto;
using Interface.IRepositories;
using Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unit_test;
using XUnitTest;

namespace Unit_Test.test
{
    public class CategoryRepositoryTests : IClassFixture<UnitOfWorkFixture>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly WebDemoDbContext _dbContext;
        private readonly CategoryRepository _repository;

        public CategoryRepositoryTests(UnitOfWorkFixture fixture)
        {
            _unitOfWork = fixture.UnitOfWork;
            _dbContext = fixture.Context;
            _repository = TestHelper.CreateRepository<CategoryRepository>(_dbContext);

            // Seed categories
            var categories = new List<Category>
        {
            new Category { Id = 1, Name = "Category1", Description = "Category Description1", CreateDate = DateTime.Now },
            new Category { Id = 2, Name = "Category2", Description = "Category Description2", CreateDate = DateTime.Now }
        };

            foreach (var category in categories)
            {
                if (_dbContext.Categories.Find(category.Id) == null)
                {
                    _dbContext.Categories.Add(category);
                }
            }

            _dbContext.SaveChanges();
        }

        [Fact]
        public void Insert_AddsNewCategory()
        {
            var newCategory = new Category
            {
                Id = 3,
                Name = "Category3",
                Description = "Category Description3",
                CreateDate = DateTime.Now 
            };

            _repository.Insert(newCategory);
            _unitOfWork.SaveChanges();

            Assert.Equal(3, _repository.GetAll().Count());
        }

        [Fact]
        public void Update_UpdatesCategory()
        {
            var categoryToUpdate = _repository.GetById(1);
            categoryToUpdate.Name = "UpdatedCategoryName";
            categoryToUpdate.Description = "UpdatedCategoryDescription";

            _repository.Update(categoryToUpdate);
            _unitOfWork.SaveChanges();

            var updatedCategory = _repository.GetById(1);
            Assert.Equal("UpdatedCategoryName", updatedCategory.Name);
            Assert.Equal("UpdatedCategoryDescription", updatedCategory.Description);
        }

        [Fact]
        public void Delete_RemovesCategory()
        {
            // Get the category to delete
            var categoryToDelete = _repository.GetById(1);
            Assert.NotNull(categoryToDelete);

            // Delete the category
            _repository.Delete(categoryToDelete);
            _unitOfWork.SaveChanges();

            // Ensure the category is deleted
            Assert.Null(_repository.GetById(1));

            // Ensure the collection has the correct number of items
            Assert.Equal(2, _repository.GetAll().Count());
        }
    }
}
