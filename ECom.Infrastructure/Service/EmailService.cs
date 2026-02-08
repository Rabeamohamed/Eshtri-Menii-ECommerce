using ECom.Core.DTO.Auth;
using ECom.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Infrastructure.Service
{
    public class EmailService : IEmailService
    {
        // Using SMTP [Simple Mail Transfer Protocol] for sending emails, using Gmail as an example, you can use any email provider that supports SMTP
        // there are other libraries like MailKit or System.Net.Mail
        public Task SendEmailAsync(EmailDto emailDto)
        {
            throw new NotImplementedException();
        }
    }
}
