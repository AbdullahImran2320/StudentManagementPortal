using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using StudentAPI.Interfaces;
using StudentAPI.Models;

namespace StudentAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress(userName, toEmail));
            message.Subject = "Welcome to Student Management System!";

            message.Body = new TextPart("html")
            {
                Text = $@"
<table role='presentation' width='100%' cellpadding='0' cellspacing='0' style='background-color:#f4f4f7; padding:30px 0;'>
  <tr>
    <td align='center'>
      <table role='presentation' width='480' cellpadding='0' cellspacing='0' style='background-color:#ffffff; border-radius:10px; overflow:hidden; box-shadow:0 4px 16px rgba(0,0,0,0.08); font-family:Segoe UI, Arial, sans-serif;'>
        
        <tr>
          <td style='background:linear-gradient(135deg,#667eea,#764ba2); padding:32px; text-align:center;'>
            <div style='font-size:36px;'>🎓</div>
            <h1 style='color:#ffffff; font-size:20px; margin:8px 0 0;'>Student Management System</h1>
          </td>
        </tr>

        <tr>
          <td style='padding:32px;'>
            <h2 style='color:#333333; font-size:20px; margin:0 0 12px;'>Welcome, {userName}! 👋</h2>
            <p style='color:#555555; font-size:15px; line-height:1.6; margin:0 0 16px;'>
              Your account has been created successfully. We're glad to have you on board.
            </p>
            <p style='color:#555555; font-size:15px; line-height:1.6; margin:0 0 24px;'>
              You can now log in and start exploring your dashboard, track student records, and manage everything in one place.
            </p>

            <table role='presentation' cellpadding='0' cellspacing='0'>
              <tr>
                <td style='border-radius:6px; background:linear-gradient(135deg,#667eea,#764ba2);'>
                  <a href='http://localhost:4200/login' target='_blank' style='display:inline-block; padding:12px 28px; color:#ffffff; text-decoration:none; font-size:15px; font-weight:600;'>
                    Go to Dashboard
                  </a>
                </td>
              </tr>
            </table>
          </td>
        </tr>

        <tr>
          <td style='padding:20px 32px; background-color:#fafafa; text-align:center; border-top:1px solid #eeeeee;'>
            <p style='color:#999999; font-size:12px; margin:0;'>
              If you didn't create this account, you can safely ignore this email.
            </p>
            <p style='color:#999999; font-size:12px; margin:8px 0 0;'>
              &copy; {DateTime.Now.Year} Student Management System
            </p>
          </td>
        </tr>

      </table>
    </td>
  </tr>
</table>"
            };


            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
                await client.SendAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", toEmail);
                // Don't rethrow — registration should still succeed even if email fails
            }
            finally
            {
                await client.DisconnectAsync(true);
            }
        }
    }
}