using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface IAuditService
    {
        /// <summary>
        /// Logs an action to the audit store.
        /// </summary>
        /// <param name="userId">ID of the user who performed the action (optional).</param>
        /// <param name="entityName">Name of the entity affected (e.g. "Order").</param>
        /// <param name="entityId">ID of the entity affected (e.g. "123").</param>
        /// <param name="action">Description of the action (e.g. "Cancelled order").</param>
        void Log(string userId, string entityName, string entityId, string action);

        /// <summary>
        /// Asynchronously logs an action.
        /// </summary>
        Task LogAsync(string userId, string entityName, string entityId, string action);
    }
}