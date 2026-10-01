namespace OnlineLearning.Business.DTOs;

public class CreateFeedbackDto
{
    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public int CourseId { get; set; }
}