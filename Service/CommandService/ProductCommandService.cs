using AutoMapper;
using Dto;
using Interface.Command;
using Interface.IRepositories;

using Webdemo.Models;

namespace Service.CommandService
{
    public class ProductCommandService : IProductCommand
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ProductCommandService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public void Delete(int id)
        {
            var prouduct = _unitOfWork.ProductRepository.FindByCondition(p => p.Id == id).SingleOrDefault();
            if (prouduct != null)
            {
                _unitOfWork.ProductRepository.Delete(prouduct);
                _unitOfWork.SaveChanges();
            }
        }

        public int Insert(ProductModel entityModel)
        {
            // Map the incoming ProductModel to the Product entity
            var productEntity = _mapper.Map<Product>(entityModel);

            // Handle the image upload (if provided)
            if (entityModel.ImageFile != null && entityModel.ImageFile.Length > 0)
            {
                // Define the path where the image will be saved
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products");

                // Ensure the directory exists
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(entityModel.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    entityModel.ImageFile.CopyTo(fileStream);
                }
                productEntity.ImagePath = "/images/products/" + uniqueFileName;
            }
            _unitOfWork.ProductRepository.Insert(productEntity);
            _unitOfWork.SaveChanges();
            return productEntity.Id;
        }

        public void Update(int id, ProductModel entityModel)
        {
            var existingProduct = _unitOfWork.ProductRepository.FindByCondition(p => p.Id == id).SingleOrDefault();
            if (existingProduct != null)
            {
                _mapper.Map(entityModel, existingProduct);
                _unitOfWork.ProductRepository.Update(existingProduct);
                _unitOfWork.SaveChanges();
            }
        }
        public void UpdateStock(int productId, int newStock)
        {
            var product = _unitOfWork.ProductRepository.FindByCondition(p => p.Id == productId).SingleOrDefault();

            if (product != null)
            {
                product.Stock = newStock;
                _unitOfWork.ProductRepository.Update(product);
                _unitOfWork.SaveChanges();
            }
        }
        public void AddReview(int productId, ReviewModel reviewModel)
        {
            var product = _unitOfWork.ProductRepository.FindByCondition(p => p.Id == productId).SingleOrDefault();

            if (product != null)
            {
                var reviewEntity = _mapper.Map<Review>(reviewModel);
                reviewEntity.ProductId = productId;
                reviewEntity.DatePosted = DateTime.UtcNow;

                // Add the review to the product
                _unitOfWork.ReviewRepository.Insert(reviewEntity);

                // Save the changes
                _unitOfWork.SaveChanges();
            }
        }
        public void ToggleAvailability(int productId)
        {
            var product = _unitOfWork.ProductRepository.FindByCondition(p => p.Id == productId).SingleOrDefault();
            if (product != null)
            {
                product.IsOutOfStock = !product.IsOutOfStock;
                _unitOfWork.ProductRepository.Update(product);
                _unitOfWork.SaveChanges();
            }
        }
        public void ToggleFeatured(int productId)
        {
            var product = _unitOfWork.ProductRepository.FindByCondition(p => p.Id == productId).SingleOrDefault();
            if (product != null)
            {
                product.IsFeatured = !product.IsFeatured;
                _unitOfWork.ProductRepository.Update(product);
                _unitOfWork.SaveChanges();
            }
        }

    }
}
