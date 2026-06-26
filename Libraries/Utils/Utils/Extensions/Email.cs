using MailKit.Net.Smtp;
using MimeKit;
using Utils.Constants;
using Utils.Models;

namespace Utils.Services
{
    public interface IEmailService
    {
        Task<bool> SendAsync(EmailMessageDo message);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> logger;

        public EmailService(
            ILogger<EmailService> logger
        )
        {
            this.logger = logger;
        }

        public async Task<bool> SendAsync(EmailMessageDo message)
        {
            try
            {
                var email = new MimeMessage();

                email.From.Add(new MailboxAddress(MAIL.MAIL_SENDER_NAME, MAIL.MAIL_SENDER));

                foreach (string to in (message.To ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
                    email.To.Add(MailboxAddress.Parse(to.Trim()));

                foreach (string cc in (message.CC ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries))
                    email.Cc.Add(MailboxAddress.Parse(cc.Trim()));

                email.Subject = message.Subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = message.Body,
                    TextBody = StripHtmlTags(message.Body),
                };

                // Attachments
                if (message.Attachments != null)
                {
                    foreach (var attachment in message.Attachments)
                    {
                        builder.Attachments.Add(attachment.Name, attachment.FileBytes, ContentType.Parse(attachment.ContentType));
                    }
                }

                email.Body = builder.ToMessageBody();

                using var client = new SmtpClient();

                client.ServerCertificateValidationCallback = (sender, certificate, chain, errors) =>
                {
                    var cn = certificate?.Subject ?? "Unknown";
                    this.logger.LogInformation("Host Server = {0}, Certificate = {1}", MAIL.MAIL_SERVER, cn);
                    return true; // You can add validation logic here if needed
                };

                //await client.ConnectAsync(MAIL.MAIL_SERVER, MAIL.MAIL_PORT ?? 587,
                //    MAIL.MAIL_SSL ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
                await client.ConnectAsync(MAIL.MAIL_SERVER, (int)MAIL.MAIL_PORT, MailKit.Security.SecureSocketOptions.SslOnConnect);
                if (!MAIL.MAIL_USE_DEFAULT_CREDENTIALS)
                {
                    await client.AuthenticateAsync(MAIL.MAIL_CREDENTAIL_USERNAME, MAIL.MAIL_CREDENTAIL_PASSWORD);
                }

                await client.SendAsync(email);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError("Cannot send Email; {0}", ex.Message);
                return false;
            }
        }

        private static string StripHtmlTags(string html)
        {
            return System.Text.RegularExpressions.Regex.Replace(html ?? "", "<.*?>", string.Empty);
        }
    }
}
