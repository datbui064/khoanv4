namespace KhoaNVCB_Client.Models
{
    public class QuestionDto
    {
        public int QuestionId { get; set; }
        public int CategoryId { get; set; }
        public string Content { get; set; } = "";
        public string OptionA { get; set; } = "";
        public string OptionB { get; set; } = "";
        public string OptionC { get; set; } = "";
        public string OptionD { get; set; } = "";
        public string CorrectAnswer { get; set; } = "";
    }
}
