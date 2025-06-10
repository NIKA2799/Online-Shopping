using AutoMapper;
using Dto;
using Interface.IRepositories;
using Interface.Model;
using Interface.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;

namespace Service.QueriesService
{
    public class CategoryQurey : ICategoryQurey
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryQurey(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IEnumerable<CategoryModel> FindAll()
        {
            var model = _unitOfWork.CategoryRepository.FindAll().ToList();
            var category = _mapper.Map<List<CategoryModel>>(model);
            return category;
        }

        public IEnumerable<CategoryModel> FindByCondition(Expression<Func<Category, bool>> predicate)
        {
            var model = _unitOfWork.CategoryRepository.FindByCondition(predicate).SingleOrDefault();
            var category = _mapper.Map<List<CategoryModel>>(model);
            return category;
        }

        public CategoryModel Get(int id)
        {
            var model = _unitOfWork.CategoryRepository.FindByCondition(c => c.Id == id).SingleOrDefault();
            var category = _mapper.Map<CategoryModel>(model);
            return category;
        }

        public IEnumerable<ProductModel> GetProductsByCategory(int categoryId)
        {
            var products = _unitOfWork.ProductCategoryRepository
             .FindByCondition(pc => pc.CategoryId == categoryId)
             .Select(pc => pc.Product)
             .ToList();
            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }
    }
}
