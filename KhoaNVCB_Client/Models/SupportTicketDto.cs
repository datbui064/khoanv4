namespace KhoaNVCB_Client.Models
{
    public class SupportTicketDto
    {
        public int TicketId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ContactInfo { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}