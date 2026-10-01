namespace OnlineLearning.Domain.Entities;

public class LessonProgress
{
    public int ProgressId { get; set; }

    public int EnrollmentId { get; set; }

    public int LessonId { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }

    // Navigation Properties
    public Enrollment Enrollment { get; set; } = null!;

    public Lesson Lesson { get; set; } = null!;
}