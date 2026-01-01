namespace CashFlow.Application.DTO.Responses
{
    public class NotificationResponse
    {
        public int NotificationId { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public DateTime SentAt { get; set; }
        public string? Status { get; set; }
    }
}