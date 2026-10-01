
namespace OnlineLearning.Business.DTOs;

public class CourseDto
{
    public int CourseId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

public string Status { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public int InstructorId { get; set; }
}

