using MailKit.Net.Smtp;
using MimeKit;

namespace Manage_System.Service
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string html);
    }   
}
