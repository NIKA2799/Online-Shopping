using AutoMapper;
using Dto;
using Interface;
using Interface.Command;
using Interface.IRepositories;
using Interface.Model;
using Moq;
using Service.CommandService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test.test
{
    public class OrderCommandTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IDiscountService> _discountServiceMock; // Added mock for IDiscountService
        private readonly Mock<IAuditService> _auditServiceMock; // Added mock for IAuditService
        private readonly OrderCommandService _orderCommand;

        public OrderCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _discountServiceMock = new Mock<IDiscountService>(); // Initialize mock
            _auditServiceMock = new Mock<IAuditService>(); // Initialize mock
            _orderCommand = new OrderCommandService(
                _unitOfWorkMock.Object,
                _mapperMock.Object,
                _discountServiceMock.Object, // Pass mock to constructor
                _auditServiceMock.Object // Pass mock to constructor
            );
        }

        // Existing test methods remain unchanged
    }
}
