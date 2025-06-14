using Dto;
using Interface.Command;
using Interface.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.CommandService
{
    public class DiscountService : IDiscountService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DiscountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Discount? GetByCode(string code)
        {
            return _unitOfWork.DiscountRepository
                .FindByCondition(d => d.Code == code)
                .FirstOrDefault();
        }

        public bool IsValid(string code)
        {
            var discount = GetByCode(code);
            return discount != null && discount.ExpirationDate > DateTime.UtcNow;
        }

        public decimal ApplyDiscount(string code, decimal totalAmount)
        {
            var discount = GetByCode(code);
            if (discount == null || discount.ExpirationDate < DateTime.UtcNow)
                return totalAmount; // No discount applied

            var discountAmount = totalAmount * (discount.DiscountPercentage / 100m);
            return totalAmount - discountAmount;
        }
    }
}