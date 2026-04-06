namespace KhoaNVCB_Client.Models
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string FullName { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime? CreatedDate { get; set; }
    }

    public class CreateCommentDto
    {
        public int PostId { get; set; }
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Website { get; set; }
        public string Content { get; set; } = "";
    }
}