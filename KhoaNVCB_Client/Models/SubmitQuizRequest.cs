public class SubmitQuizRequest
{
    public int AccountId { get; set; }
    public int CategoryId { get; set; }
    public Dictionary<int, string> Answers { get; set; } = new();
}