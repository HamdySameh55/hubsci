namespace OnlineLearning.Domain.Entities;

public class Answer
{
    public int AnswerId { get; set; }

    public string AnswerText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }

    // Navigation Property
    public Question Question { get; set; } = null!;
}