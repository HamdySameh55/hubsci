namespace OnlineLearning.Domain.Entities;

public class QuizAttempt
{
    public int QuizAttemptId { get; set; }

    public int EnrollmentId { get; set; }

    public int QuizId { get; set; }

    public decimal Score { get; set; }

    public DateTime AttemptDate { get; set; }

    public bool Passed { get; set; }

    public int AttemptNumber { get; set; }

    // Navigation Properties
    public Enrollment Enrollment { get; set; } = null!;

    public Quiz Quiz { get; set; } = null!;
}