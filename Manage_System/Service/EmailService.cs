using Manage_System.ModelViews;
using MimeKit;
using Org.BouncyCastle.Crypto.Macs;
using System.Net;
using System.Net.Mail;

namespace Manage_System.Service
{
    public class EmailService : IEmailService
    {

		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

        public async Task<bool> SendEmailAsync(string to, string subject, string html)
        {
			bool status = false;
			try
			{

				MailMessage mailMessage = new MailMessage()
                {
					From = new MailAddress("chauvanphuc9102@gmail.com"),
					Subject = subject,
					Body = html
				};
				mailMessage.To.Add(to);
				SmtpClient smtpClient = new SmtpClient("smtp.gmail.com") 
				{ 
					Port = 587,
					Credentials = new NetworkCredential("chauvanphuc9102@gmail.com", "vqgd cpip xkjn popm"),
                    EnableSsl = true
				};

				await smtpClient.SendMailAsync(mailMessage);
				status = true;

			}
			catch	
			{

                status = false;
            }

			return status;
        }
    }
}
