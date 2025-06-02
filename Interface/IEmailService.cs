using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface
{
    public interface IEmailConfiguration
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
