namespace OnlineLearning.Business.DTOs;

public class CourseProgressDto
{
    public int EnrollmentId { get; set; }
    public int CourseId { get; set; }

    public int TotalLessons { get; set; }
    public int CompletedLessons { get; set; }

    public int TotalQuizzes { get; set; }
    public int PassedQuizzes { get; set; }

    public decimal ProgressPercentage { get; set; }

    public bool IsCompleted { get; set; }
}