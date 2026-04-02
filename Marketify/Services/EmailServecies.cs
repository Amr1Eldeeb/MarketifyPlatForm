using MailKit.Security;
using Marketify.Settings;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace Marketify.Services
{
    public class EmailServecies(IOptions<MailSettings> mailsettings, ILogger<EmailServecies> logger) : IEmailSender
    {
       
        private readonly MailSettings _mailSettings = mailsettings.Value; //to read confgs from appSettings

        public ILogger<EmailServecies> _logger = logger;
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage
            {
                Sender = MailboxAddress.Parse(_mailSettings.Mail), // sender email
                Subject = subject,

            };
            message.To.Add(MailboxAddress.Parse(email));
            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage ///body of email 
            };
            _logger.LogInformation("Sending Email to {email} ", email);
            message.Body = builder.ToMessageBody(); //convert to message of body
            using var smtp = new SmtpClient();//for mail kit
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls); //connect ons sevrver 
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            //send message 
            await smtp.SendAsync(message);
            smtp.Disconnect(true);
        }
    } 



}
    
    
