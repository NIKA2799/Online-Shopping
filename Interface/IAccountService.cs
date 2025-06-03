using Interface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface IAccountService
    {
        Task<(bool Success, string? Error, object? Result)> RegisterAsync(RegisterModel model, Func<string, string, object, string> urlAction);
        Task<(bool Success, string Error)> ConfirmEmailAsync(string userId, string token);
        Task<(bool Success, string? Error, object? Result)> LoginAsync(LoginModel model);
    }
}

