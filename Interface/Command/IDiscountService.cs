using Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Command
{
    public interface IDiscountService
    {
        Discount? GetByCode(string code);
        bool IsValid(string code);
        decimal ApplyDiscount(string code, decimal totalAmount);
    }
}
