namespace OnlineLearning.Domain.Entities;

public class Feedback
{
    public int FeedbackId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }

    public int CourseId { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;

    public Course Course { get; set; } = null!;
}