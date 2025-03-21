using AutoMapper;
using Dto;
using Interface.Command;
using Interface.IRepositories;
using Interface.Model;

namespace Service.CommandService
{
    public class CategoryCommand : ICategoryCommand
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryCommand(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public void Delete(int id)
        {
            var category = _unitOfWork.CategoryRepository.FindByCondition(c => c.Id == c.Id).SingleOrDefault();
            if(category != null)
            {
                _unitOfWork.CategoryRepository.Delete(category);
                _unitOfWork.SaveChanges();
            }
        }

        public int Insert(CategoryModel entityModel)
        {
            var category = _mapper.Map<Category>(entityModel);
            _unitOfWork.CategoryRepository.Insert(category);
            _unitOfWork.SaveChanges();
            return category.Id;
        }

        public void Update(int id, CategoryModel entityModel)
        {
            var category = _unitOfWork.CategoryRepository.FindByCondition(c => c.Id == id).SingleOrDefault();
            if(category != null)
            {
                var updatcategory = _mapper.Map<Category>(entityModel);
                updatcategory.Id = category.Id;
                _unitOfWork.CategoryRepository.Update(updatcategory);
                _unitOfWork.SaveChanges();
            }
        }
    }
}
