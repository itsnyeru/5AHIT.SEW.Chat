using System.Net;
using System.Net.Mail;
using System.Text;

namespace Services {
    public class MailSettings {
        public string Displayname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Host { get; set; }
    }

    public interface IMailService {
        Task SendEmailAsync(string ToEmail, string Subject, string Body);
    }

    public class MailService : IMailService {
        private readonly MailSettings _mailConfig;

        public MailService(MailSettings mailConfig) {
            _mailConfig = mailConfig;
        }

        public async Task SendEmailAsync(string ToEmail, string Subject, string Body) {
            using (SmtpClient smtp = new SmtpClient()) {
                smtp.Port = _mailConfig.Port;
                smtp.Host = _mailConfig.Host;
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(_mailConfig.Username, _mailConfig.Password);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                MailMessage message = new MailMessage() {
                    From = new MailAddress(_mailConfig.Username, _mailConfig.Displayname),
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8,
                    HeadersEncoding = Encoding.UTF8,
                    Priority = MailPriority.High,
                    Subject = Subject,
                    IsBodyHtml = true,
                    Body = Body,
                };
                message.To.Add(new MailAddress(ToEmail));
            
                await smtp.SendMailAsync(message);
            }
                
        }
    }
}