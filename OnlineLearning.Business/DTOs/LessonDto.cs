
namespace OnlineLearning.Business.DTOs;

public class LessonDto
{
    public int LessonId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public string ContentUrl { get; set; } = string.Empty;

    public int ModuleId { get; set; }

    public int CourseId { get; set; }
}

