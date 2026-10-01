using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface IEnrollmentRepository
{
    Task<IEnumerable<Enrollment>> GetAllAsync();

    Task<IEnumerable<Enrollment>> GetByUserIdAsync(int userId);

    Task<Enrollment?> GetByIdAsync(int id);

    Task AddAsync(Enrollment enrollment);

    Task<Enrollment?> GetByUserAndCourseAsync(
        int userId,
        int courseId);

    Task<bool> HasActiveEnrollmentAsync(
        int userId,
        int courseId);

    Task<Enrollment?> GetActiveEnrollmentAsync(
        int userId,
        int courseId);

    void Update(Enrollment enrollment);

    void Delete(Enrollment enrollment);

    Task SaveChangesAsync();
}