namespace OnlineLearning.Business.DTOs;

public class QuizDto
{
    public int QuizId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int ModuleId { get; set; }

    public int CourseId { get; set; }

    public List<QuestionDto> Questions { get; set; } = new();
}