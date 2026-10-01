namespace OnlineLearning.Business.DTOs;

public class AnswerDto
{
    public int AnswerId { get; set; }

    public string AnswerText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }
}