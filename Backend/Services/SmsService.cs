using Microsoft.Extensions.Options;
using StudentAPI.Interfaces;
using StudentAPI.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace StudentAPI.Services
{
    public class SmsService : ISmsService
    {
        private readonly SmsSettings _settings;
        private readonly ILogger<SmsService> _logger;

        public SmsService(IOptions<SmsSettings> settings, ILogger<SmsService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
        }

        public async Task SendWelcomeSmsAsync(string toPhoneNumber, string userName)
        {
            try
            {
                await MessageResource.CreateAsync(
                    body: $"Welcome, {userName}! Your Student Management System account is ready.",
                    from: new PhoneNumber(_settings.TwilioPhoneNumber),
                    to: new PhoneNumber(toPhoneNumber)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome SMS to {Phone}", toPhoneNumber);
                // Don't rethrow — registration should still succeed even if SMS fails
            }
        }
    }
}