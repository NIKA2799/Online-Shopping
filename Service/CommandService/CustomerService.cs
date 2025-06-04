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
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public CustomerModel GetCustomerById(int id)
        {
            var entity = _unitOfWork.CustomerRepository.GetById(id);
            return _mapper.Map<CustomerModel>(entity);
        }

        public IEnumerable<CustomerModel> GetAllCustomers()
        {
            var customers = _unitOfWork.CustomerRepository.GetAll();
            return _mapper.Map<IEnumerable<CustomerModel>>(customers);
        }

        public async Task<Customer> CreateCustomerAsync(RegisterModel model, string applicationUserId)
        {
            var customer = new Customer
            {
                Name = model.Name,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                ShippingAddress = model.ShippingAddress,
                BillingAddress = model.BillingAddress,
                ApplicationUserId = applicationUserId,
                DateCreated = DateTime.UtcNow
            };

            _unitOfWork.CustomerRepository.Insert(customer);
            await _unitOfWork.SaveChangesAsync();
            return customer;
        }


        public void UpdateCustomer(int id, CustomerModel model)
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

        // ✅ პროდუქტის ატვირთვა მომხმარებლის მიერ
        public int UploadProduct(ProductModel productModel, int customerId)
        {
            var customer = _unitOfWork.CustomerRepository.GetById(customerId);
            if (customer == null)
                throw new Exception("Customer not found.");

            var product = _mapper.Map<Product>(productModel);
            product.CreateDate = DateTime.UtcNow;
            product.IsOutOfStock = false;
            product.IsFeatured = false;

            // 📌 დავაკავშიროთ მომხმარებელთან
            product.UserId = customerId;

            _unitOfWork.ProductRepository.Insert(product);
            _unitOfWork.SaveChanges();
            return product.Id;
        }

        // ✅ მომხმარებლის ატვირთული პროდუქტების ნახვა
        public IEnumerable<ProductModel> GetMyProducts(int customerId)
        {
            var products = _unitOfWork.ProductRepository
                .FindByCondition(p => p.UserId == customerId)
                .ToList();

            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }
    }
}
