namespace OnlineLearning.Business.DTOs;

public class CreateQuizAttemptDto
{
    public int EnrollmentId { get; set; }

    public int QuizId { get; set; }

    public List<QuizAnswerSelectionDto> Answers { get; set; } = new();
}