using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface ILessonRepository
{
    Task<IEnumerable<Lesson>> GetAllAsync();

    Task<Lesson?> GetByIdAsync(int id);

    Task<Module?> GetModuleByLessonIdAsync(
        int lessonId);

    Task<IEnumerable<Lesson>> GetByCourseIdAsync(
        int courseId);

    Task<bool> HasProgressAsync(int lessonId);

    Task AddAsync(Lesson lesson);

    void Update(Lesson lesson);

    void Delete(Lesson lesson);

    Task SaveChangesAsync();
}