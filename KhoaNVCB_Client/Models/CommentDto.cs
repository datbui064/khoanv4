namespace KhoaNVCB_Client.Models
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int AccountId { get; set; }
        public string FullName { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
    }

    public class CreateCommentDto
    {
        public int PostId { get; set; }
        public int AccountId { get; set; }
        public string Content { get; set; } = null!;
    }
}