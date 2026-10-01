namespace OnlineLearning.Domain.Entities;

public class Course
{
    public int CourseId { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int InstructorId { get; set; }
    public string Status { get; set; } = "Draft";
 
    public User Instructor { get; set; } = null!;

    public ICollection<Module> Modules { get; set; } = new List<Module>();

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}