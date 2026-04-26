using ECom.Application.DTO.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECom.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailDto emailDto);
    }
}
