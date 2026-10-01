
namespace OnlineLearning.Business.DTOs;

public class CertificateDto
{
    public int CertificateId { get; set; }

    public DateTime IssueDate { get; set; }

    public string CertificateUrl { get; set; } = string.Empty;

    public int EnrollmentId { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string CourseTitle { get; set; } = string.Empty;
}
