using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface IPaymentRepository
{
    Task<IEnumerable<Payment>> GetAllAsync();

    Task<Payment?> GetByIdAsync(int id);

    Task<Payment?> GetByEnrollmentIdAsync(int enrollmentId);

    Task<Payment?> GetByIdWithEnrollmentAsync(int id);

    Task<IEnumerable<Payment>> GetPendingForInstructorAsync(
        int instructorId);

    Task AddAsync(Payment entity);

    void Update(Payment entity);

    void Delete(Payment entity);

    Task SaveChangesAsync();
}