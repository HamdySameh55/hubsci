
namespace OnlineLearning.Business.DTOs;

public class CreateQuestionDto
{
    public string QuestionText { get; set; } = string.Empty;

    public int QuizId { get; set; }
}

