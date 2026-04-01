namespace KhoaNVCB_Client.Models
{
    public class QuizResultDto
    {
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public string Message { get; set; } = "";
    }
}
