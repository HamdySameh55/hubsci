namespace OnlineLearning.Domain.Entities;

public class Payment
{
    public int PaymentId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaymentDate { get; set; }

public string PaymentStatus { get; set; } = "Pending";

    public string PaymentMethod { get; set; } = string.Empty;

    public string? TransactionReference { get; set; }

    public string? ProofFilePath { get; set; }

    public int EnrollmentId { get; set; }

    // Navigation Property
    public Enrollment Enrollment { get; set; } = null!;
}