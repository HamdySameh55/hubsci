namespace OnlineLearning.Business.DTOs;

public class PaymentDto
{
    public int PaymentId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public int EnrollmentId { get; set; }

    public string? TransactionReference { get; set; }

    public string? ProofFilePath { get; set; }

    public string StudentName { get; set; } = string.Empty;

    public string CourseTitle { get; set; } = string.Empty;

    public int CourseId { get; set; }
}
