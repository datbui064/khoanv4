namespace KhoaNVCB_Client.Models // Thay đổi theo namespace của bạn
{
    public class TopicDto
    {
        public int TopicId { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string? FileUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public string? UserId { get; set; }
        public string? AuthorName { get; set; }
    }
}