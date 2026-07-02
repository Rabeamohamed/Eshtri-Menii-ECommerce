using System;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace TempEmailTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var Smtp = "smtp.gmail.com";
            var Port = 465;
            var From = "rabea2mohamed@gmail.com";
            var UserName = "rabea2mohamed@gmail.com";
            var Password = "adumebctoxicvihw";
            var To = "rabea2mohamed@gmail.com";
            var Subject = "Test Subject";
            var Content = "Test Content";

            MimeMessage message = new();
            message.From.Add(new MailboxAddress("Eshtri-Menii", From));
            message.Subject = Subject;
            message.To.Add(new MailboxAddress(To, To));
            message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = Content
            };

            using (var smtp = new SmtpClient())
            {
                try
                {
                    Console.WriteLine("Connecting...");
                    await smtp.ConnectAsync(Smtp, Port, true);
                    
                    Console.WriteLine("Authenticating...");
                    await smtp.AuthenticateAsync(UserName, Password);
                    
                    Console.WriteLine("Sending...");
                    await smtp.SendAsync(message);

                    Console.WriteLine("Sent Successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.GetType().Name}");
                    Console.WriteLine($"Message: {ex.Message}");
                }
                finally
                {
                    await smtp.DisconnectAsync(true);
                }
            }
        }
    }
}
