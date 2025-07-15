using Dto;
using Interface.IRepositories;
using Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AuditService : IAuditService
    {
        private readonly IUnitOfWork _uow;

        public AuditService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public void Log(string userId, string entityName, string entityId, string action)
        {
            var entry = new AuditLog
            {
                UserId = userId,
                EntityName = entityName,
                EntityId = entityId,
                Action = action
            };

            _uow.AuditLogRepository.Insert(entry);
            _uow.SaveChanges();
        }

        public async Task LogAsync(string userId, string entityName, string entityId, string action)
        {
            var entry = new AuditLog
            {
                UserId = userId,
                EntityName = entityName,
                EntityId = entityId,
                Action = action
            };

            _uow.AuditLogRepository.Insert(entry);
            await _uow.SaveChangesAsync();
        }
    }
}