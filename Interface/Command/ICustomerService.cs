using Dto;
using Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdemo.Models;

namespace Interface.Command
{
    public interface ICustomerService
    {
        UserModel GetCustomerById(int id);
        IEnumerable<UserModel> GetAllCustomers();
        Task<User> CreateCustomerAsync(RegisterModel model, string applicationUserId);
        void UpdateCustomer(int id, UserModel model);
        void DeleteCustomer(int id);

        // 💡 გაყიდვისთვის საჭირო
        int UploadProduct(ProductModel productModel, int customerId);
        IEnumerable<ProductModel> GetMyProducts(int customerId);

    }
}
