
namespace OnlineLearning.Business.DTOs;

public class FeedbackDto
{
    public int FeedbackId { get; set; }

    public int Rating { get; set; }
    public string Status { get; set; } = "Pending";

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public int UserId { get; set; }

    public int CourseId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string CourseTitle { get; set; } = string.Empty;
}

