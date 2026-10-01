namespace OnlineLearning.Business.DTOs;

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }

    public int TotalStudents { get; set; }

    public int TotalInstructors { get; set; }

    public int TotalCourses { get; set; }

    public int PublishedCourses { get; set; }

    public int TotalEnrollments { get; set; }

    public decimal TotalRevenue { get; set; }
}