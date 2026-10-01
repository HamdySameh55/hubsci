namespace OnlineLearning.Business.DTOs;

public class QuestionDto
{
    public int QuestionId { get; set; }

    public string QuestionText { get; set; } = string.Empty;

    public int QuizId { get; set; }

    public List<QuizAnswerDto> Answers { get; set; } = new();
}