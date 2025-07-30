using AutoMapper;
using Dto;
using Interface.Command;
using Interface.IRepositories;
using Interface.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;

namespace Service.CommandService
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public UserModel GetCustomerById(int id)
        {
            var entity = _unitOfWork.CustomerRepository.GetById(id);
            return _mapper.Map<UserModel>(entity);
        }

        public IEnumerable<UserModel> GetAllCustomers()
        {
            var customers = _unitOfWork.CustomerRepository.GetAll();
            return _mapper.Map<IEnumerable<UserModel>>(customers);
        }

        public async Task<User> CreateCustomerAsync(RegisterModel model, string applicationUserId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Map and hash the password (your User.Password setter will BCrypt it for you)
            var customer = new User
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                ShippingAddress = model.ShippingAddress,
                BillingAddress = model.BillingAddress,
                Password = model.Password,       // <-- triggers BCrypt.HashPassword
                ApplicationUserId = applicationUserId,
                DateCreated = DateTime.UtcNow
            };

            // Persist
            _unitOfWork.CustomerRepository.Insert(customer);
            await _unitOfWork.SaveChangesAsync();

            return customer;
        }
        


        public void UpdateCustomer(int id, UserModel model)
        {
            var existing = _unitOfWork.CustomerRepository.GetById(id);
            if (existing != null)
            {
                _mapper.Map(model, existing);
                _unitOfWork.CustomerRepository.Update(existing);
                _unitOfWork.SaveChanges();
            }
        }

        public void DeleteCustomer(int id)
        {
            var customer = _unitOfWork.CustomerRepository.GetById(id);
            if (customer != null)
            {
                _unitOfWork.CustomerRepository.Delete(customer);
                _unitOfWork.SaveChanges();
            }
        }

       
        public int UploadProduct(ProductModel productModel, int customerId)
        {
            var customer = _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer == null)
                throw new Exception("Customer not found.");

            var product = _mapper.Map<Product>(productModel);
            product.CreateDate = DateTime.UtcNow;
            product.IsOutOfStock = false;
            product.IsFeatured = false;

         
            product.UserId = customerId;

            _unitOfWork.ProductRepository.Insert(product);
            _unitOfWork.SaveChanges();
            return product.Id;
        }

      
        public IEnumerable<ProductModel> GetMyProducts(int customerId)
        {
            var products = _unitOfWork.ProductRepository
                .FindByCondition(p => p.UserId == customerId)
                .ToList();

            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }
    }
}
