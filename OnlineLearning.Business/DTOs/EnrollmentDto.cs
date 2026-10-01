
namespace OnlineLearning.Business.DTOs;

public class EnrollmentDto
{
    public int EnrollmentId { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public int UserId { get; set; }

    public int CourseId { get; set; }
}

