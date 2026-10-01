
namespace OnlineLearning.Business.DTOs;

public class QuizAttemptDto
{
    public int QuizAttemptId { get; set; }

    public int EnrollmentId { get; set; }

    public int QuizId { get; set; }

    public decimal Score { get; set; }

    public DateTime AttemptDate { get; set; }

    public bool Passed { get; set; }

    public int AttemptNumber { get; set; }
}

