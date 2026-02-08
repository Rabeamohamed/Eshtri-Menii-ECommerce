using ECom.Core.DTO.Auth;
using ECom.Core.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
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

        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(EmailDto emailDto)
        {
            MimeMessage message = new ();
            message.From.Add (new MailboxAddress ("Eshtri-Menii",_configuration["EmailSettings:From"]));
            message.Subject = emailDto.Subject;
            message.To.Add (new MailboxAddress (emailDto.To, emailDto.To));
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = emailDto.Content
            };
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await smtp.ConnectAsync(_configuration["EmailSettings:Smtp"],
                        int.Parse(_configuration["EmailSettings:Port"]),true);

                    await smtp.AuthenticateAsync(_configuration["EmailSettings:UserName"],
                        _configuration["EmailSettings:Password"]);

                    await smtp.SendAsync(message);
                }
                catch (Exception ex)
                {

                    throw;
                }
                finally
                {
                    //await smtp.DisconnectAsync(true);
                    smtp.DisconnectAsync(true);
                    smtp.Dispose();
                }
            }
        }
    }
}
