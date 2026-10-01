namespace OnlineLearning.Domain.Entities;

public class Lesson
{
    public int LessonId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public string ContentUrl { get; set; } = string.Empty;

    public int ModuleId { get; set; }

    // Navigation Properties
    public Module Module { get; set; } = null!;

    public ICollection<LessonProgress> ProgressRecords { get; set; }
        = new List<LessonProgress>();
}