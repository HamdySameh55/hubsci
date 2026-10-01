namespace OnlineLearning.Domain.Entities;

public class Certificate
{
    public int CertificateId { get; set; }

    public DateTime IssueDate { get; set; }

    public string CertificateUrl { get; set; } = string.Empty;

    public int EnrollmentId { get; set; }

    // Navigation Property
    public Enrollment Enrollment { get; set; } = null!;
}