namespace OnlineLearning.Domain.Entities;

public class Question
{
    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public int QuizId { get; set; }

    // Navigation Properties
    public Quiz Quiz { get; set; } = null!;

    public ICollection<Answer> Answers { get; set; }
        = new List<Answer>();
}