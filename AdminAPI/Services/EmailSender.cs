using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace AdminAPI.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfigurationRoot _config;

        public EmailSender()
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            _config = builder.Build();
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            await SendDebugEmail(toEmail, subject, htmlMessage);
        }

        private async Task SendDebugEmail(string toEmail, string subject, string htmlMessage)
        {
            var sender = new SmtpSender(() => new SmtpClient("localhost")
            {
                EnableSsl = false,
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = @"C:\Users\Tomi\Documents\ELTE-IK\Szakdolgozat\DebugMails"
            });

            Email.DefaultSender = sender;

            var email = await Email
                .From("test@email.confirm.test")
                .To(toEmail)
                .Subject(subject)
                .Body(htmlMessage)
                .SendAsync();
        }

        private async Task SendRealEmail(string toEmail, string subject, string htmlMessage)
        {
            SmtpClient client = new SmtpClient(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]!))
            {
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential(_config["Smtp:Username"], _config["Smtp:Password"]),
            };

            var sender = new SmtpSender(client);

            Email.DefaultSender = sender;


            var email = await Email
                .From(_config["Smtp:Username"])
                .To(toEmail)
                .Subject(subject)
                .Body(htmlMessage)
                .SendAsync();

            if (email.Successful)
            {
                Console.WriteLine("Email sent successfully");
            }
            else
            {
                Console.WriteLine("Email sending unsuccessful");
            }
        }
    }
}
