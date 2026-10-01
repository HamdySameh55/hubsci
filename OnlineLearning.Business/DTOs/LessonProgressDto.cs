namespace OnlineLearning.Business.DTOs;

public class LessonProgressDto
{
    public int ProgressId { get; set; }

    public int EnrollmentId { get; set; }

    public int LessonId { get; set; }

    public string LessonTitle { get; set; } = string.Empty;

    public string CourseTitle { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }
}