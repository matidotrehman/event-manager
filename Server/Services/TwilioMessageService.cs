using EventManager.Server.Controllers.TwilioMessage;
using EventManager.Server.Interface;
using EventManager.Server.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.TwiML.Messaging;

namespace EventManager.Server.Services
{
    public class TwilioMessageService : ITwilioMessageService
    {
        private readonly IConfiguration _configuration;

        public TwilioMessageService(IConfiguration configuration)
        {
            _configuration = configuration;
            TwilioClient.Init(_configuration["Twilio:AccountSid"], _configuration["Twilio:AuthToken"]);
        }

        public async Task<bool> SendSmsAsync(SmsRequest request)
        {
            try
            {
                await MessageResource.CreateAsync(
                    body: request.Message,
                    from: new Twilio.Types.PhoneNumber(_configuration["Twilio:TwilioPhoneNumber"]),
                    to: new Twilio.Types.PhoneNumber(request.PhoneNumber)
                    );

                return true;
            }
            catch (Exception ex)
            {
                // Handle any exceptions or logging here if necessary.
                throw(ex);
            }
        }
    }

}
