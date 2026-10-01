namespace OnlineLearning.Domain.Entities;

public class User
{
    public int UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

 
    public ICollection<Course> Courses { get; set; } = new List<Course>();

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}