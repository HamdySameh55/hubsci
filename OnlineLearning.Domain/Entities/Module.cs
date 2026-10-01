namespace OnlineLearning.Domain.Entities;

public class Module
{
    public int ModuleId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int OrderNumber { get; set; }

    public int CourseId { get; set; }

    // Navigation Properties
    public Course Course { get; set; } = null!;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

    public Quiz? Quiz { get; set; }
}