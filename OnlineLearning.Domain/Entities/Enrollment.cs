namespace OnlineLearning.Domain.Entities;

public class Enrollment
{
    public int EnrollmentId { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public string Status { get; set; } = "Pending";

    public int UserId { get; set; }

    public int CourseId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;

    public Course Course { get; set; } = null!;

public Payment Payment { get; set; } = null!;

    public Certificate? Certificate { get; set; }

    public ICollection<LessonProgress> LessonProgresses { get; set; }
        = new List<LessonProgress>();

    public ICollection<QuizAttempt> QuizAttempts { get; set; }
        = new List<QuizAttempt>();
}