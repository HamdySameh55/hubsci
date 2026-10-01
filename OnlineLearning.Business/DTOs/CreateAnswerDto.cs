namespace OnlineLearning.Business.DTOs;

public class CreateAnswerDto
{
    public string AnswerText { get; set; } = string.Empty;

    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }
}