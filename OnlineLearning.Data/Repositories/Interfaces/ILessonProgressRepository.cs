using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface ILessonProgressRepository
{
    Task<IEnumerable<LessonProgress>> GetAllAsync();

    Task<LessonProgress?> GetByIdAsync(int id);

    Task<LessonProgress?> GetByIdForUserAsync(
        int id,
        int userId);

    Task<IEnumerable<LessonProgress>> GetByEnrollmentIdAsync(
        int enrollmentId);

    Task<IEnumerable<LessonProgress>> GetByUserIdAsync(
        int userId);

    Task AddAsync(LessonProgress progress);

    void Update(LessonProgress progress);

    void Delete(LessonProgress progress);

    Task SaveChangesAsync();
}