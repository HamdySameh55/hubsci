namespace OnlineLearning.Business.DTOs;

public class CreatePaymentDto
{
    public decimal Amount { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;

    public string PaymentMethod { get; set; } = string.Empty;

    public int EnrollmentId { get; set; }
}