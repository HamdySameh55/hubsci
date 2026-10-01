using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IEnrollmentRepository enrollmentRepository)
    {
        _paymentRepository = paymentRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<IEnumerable<PaymentDto>> GetAllAsync()
    {
        var payments =
            await _paymentRepository.GetAllAsync();

        return payments.Select(payment => MapToDto(payment));
    }

    public async Task<PaymentDto?> GetByIdAsync(int id)
    {
        var payment =
            await _paymentRepository
                .GetByIdWithEnrollmentAsync(id);

        if (payment == null)
            return null;

        return MapToDto(payment);
    }

    public async Task<PaymentDto?> GetByEnrollmentIdAsync(
        int enrollmentId)
    {
        var payment =
            await _paymentRepository
                .GetByEnrollmentIdAsync(enrollmentId);

        if (payment == null)
            return null;

        var full =
            await _paymentRepository
                .GetByIdWithEnrollmentAsync(payment.PaymentId);

        return full == null ? null : MapToDto(full);
    }

    public async Task<PaymentDto> CreatePendingAsync(
        int enrollmentId,
        int userId)
    {
        var enrollment =
            await _enrollmentRepository.GetByIdAsync(enrollmentId);

        if (enrollment == null)
            throw new KeyNotFoundException(
                "Enrollment not found.");

        if (enrollment.UserId != userId)
            throw new UnauthorizedAccessException(
                "You are not allowed to access this enrollment.");

        if (enrollment.Status == "Active")
            throw new InvalidOperationException(
                "Enrollment is already active.");

        if (enrollment.Course == null)
            throw new KeyNotFoundException(
                "Course not found.");

        if (enrollment.Course.Price <= 0)
            throw new InvalidOperationException(
                "This is a free course. No payment is required.");

        var existing =
            await _paymentRepository
                .GetByEnrollmentIdAsync(enrollmentId);

        if (existing != null)
        {
            if (existing.PaymentStatus == "Successful")
                throw new InvalidOperationException(
                    "Payment has already been verified.");

            if (existing.PaymentStatus == "Pending" &&
                existing.TransactionReference != null)
            {
                throw new InvalidOperationException(
                    "Payment proof has already been submitted. Waiting for instructor verification.");
            }

            if (existing.PaymentStatus == "Failed")
            {
                existing.PaymentStatus = "Pending";
                existing.TransactionReference = null;
                existing.ProofFilePath = null;

                _paymentRepository.Update(existing);
                await _paymentRepository.SaveChangesAsync();

                return MapToDto(existing, enrollment);
            }

            return MapToDto(existing, enrollment);
        }

        var payment = new Payment
        {
            Amount = enrollment.Course.Price,
            PaymentDate = DateTime.UtcNow,
            PaymentStatus = "Pending",
            PaymentMethod = "InstaPay",
            EnrollmentId = enrollmentId
        };

        await _paymentRepository.AddAsync(payment);
        await _paymentRepository.SaveChangesAsync();

        return MapToDto(payment, enrollment);
    }

    public async Task<PaymentDto> SubmitProofAsync(
        int paymentId,
        string transactionReference,
        string proofFilePath,
        int userId)
    {
        var payment =
            await _paymentRepository
                .GetByIdWithEnrollmentAsync(paymentId);

        if (payment == null)
            throw new KeyNotFoundException(
                "Payment not found.");

        if (payment.Enrollment == null)
            throw new KeyNotFoundException(
                "Enrollment not found.");

        if (payment.Enrollment.UserId != userId)
            throw new UnauthorizedAccessException(
                "You are not allowed to modify this payment.");

        if (payment.PaymentStatus != "Pending")
            throw new InvalidOperationException(
                "This payment cannot be updated.");

        if (string.IsNullOrWhiteSpace(transactionReference))
            throw new ArgumentException(
                "Transaction reference is required.");

        payment.TransactionReference =
            transactionReference.Trim();

        payment.ProofFilePath = proofFilePath;

        _paymentRepository.Update(payment);
        await _paymentRepository.SaveChangesAsync();

        return MapToDto(payment);
    }

    public async Task<IEnumerable<PaymentDto>>
        GetPendingForInstructorAsync(int instructorId)
    {
        var payments =
            await _paymentRepository
                .GetPendingForInstructorAsync(instructorId);

        return payments.Select(payment => MapToDto(payment));
    }

    public async Task ApproveAsync(
        int paymentId,
        int instructorId)
    {
        var payment =
            await _paymentRepository
                .GetByIdWithEnrollmentAsync(paymentId);

        if (payment == null)
            throw new KeyNotFoundException(
                "Payment not found.");

        if (payment.PaymentStatus != "Pending")
            throw new InvalidOperationException(
                "Only pending payments can be approved.");

        if (payment.Enrollment == null)
            throw new KeyNotFoundException(
                "Enrollment not found.");

        if (payment.Enrollment.Course == null)
            throw new KeyNotFoundException(
                "Course not found.");

        if (payment.Enrollment.Course.InstructorId != instructorId)
            throw new UnauthorizedAccessException(
                "You can only approve payments for your own courses.");

        payment.PaymentStatus = "Successful";

        _paymentRepository.Update(payment);

        payment.Enrollment.Status = "Active";

        _enrollmentRepository.Update(payment.Enrollment);

        await _paymentRepository.SaveChangesAsync();
    }

    public async Task RejectAsync(
        int paymentId,
        int instructorId)
    {
        var payment =
            await _paymentRepository
                .GetByIdWithEnrollmentAsync(paymentId);

        if (payment == null)
            throw new KeyNotFoundException(
                "Payment not found.");

        if (payment.PaymentStatus != "Pending")
            throw new InvalidOperationException(
                "Only pending payments can be rejected.");

        if (payment.Enrollment == null)
            throw new KeyNotFoundException(
                "Enrollment not found.");

        if (payment.Enrollment.Course == null)
            throw new KeyNotFoundException(
                "Course not found.");

        if (payment.Enrollment.Course.InstructorId != instructorId)
            throw new UnauthorizedAccessException(
                "You can only reject payments for your own courses.");

        payment.PaymentStatus = "Failed";

        _paymentRepository.Update(payment);

        await _paymentRepository.SaveChangesAsync();
    }

    private static PaymentDto MapToDto(
        Payment payment,
        Enrollment? enrollment = null)
    {
        var e = enrollment ?? payment.Enrollment;

        return new PaymentDto
        {
            PaymentId = payment.PaymentId,
            Amount = payment.Amount,
            PaymentDate = payment.PaymentDate,
            PaymentStatus = payment.PaymentStatus,
            PaymentMethod = payment.PaymentMethod,
            EnrollmentId = payment.EnrollmentId,
            TransactionReference = payment.TransactionReference,
            ProofFilePath = payment.ProofFilePath,
            StudentName = e?.User?.Name ?? string.Empty,
            CourseTitle = e?.Course?.Title ?? string.Empty,
            CourseId = e?.CourseId ?? 0
        };
    }
}
