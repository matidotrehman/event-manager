using EventManager.Server.Controllers.TwilioMessage;
using EventManager.Server.Models;

namespace EventManager.Server.Interface
{
    public interface ITwilioMessageService
    {
        Task<bool> SendSmsAsync(SmsRequest request);
    }
}
