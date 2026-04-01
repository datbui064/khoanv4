namespace KhoaNVCB_Client.Models
{
    public class QuizSettingDto
    {
        public int SettingId { get; set; }
        public int CategoryId { get; set; }
        public int QuestionCount { get; set; } = 20;
        public int TimeLimitMinutes { get; set; } = 15;
    }
}
