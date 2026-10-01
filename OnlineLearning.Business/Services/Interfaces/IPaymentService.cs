using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetAllAsync();

    Task<PaymentDto?> GetByIdAsync(int id);

    Task<PaymentDto?> GetByEnrollmentIdAsync(int enrollmentId);

    Task<PaymentDto> CreatePendingAsync(
        int enrollmentId,
        int userId);

    Task<PaymentDto> SubmitProofAsync(
        int paymentId,
        string transactionReference,
        string proofFilePath,
        int userId);

    Task<IEnumerable<PaymentDto>> GetPendingForInstructorAsync(
        int instructorId);

    Task ApproveAsync(
        int paymentId,
        int instructorId);

    Task RejectAsync(
        int paymentId,
        int instructorId);
}
