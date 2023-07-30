namespace EventManager.Server.Models
{
    public class SmsRequest
    {
        public string Message { get; set; }
        public string PhoneNumber { get; set; }
        public string EventId { get; set; }
    }
}
