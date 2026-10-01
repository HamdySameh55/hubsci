using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface ICourseRepository
{
    Task<IEnumerable<Course>> GetAllAsync();

    Task<Course?> GetByIdAsync(int id);

    Task<bool> HasEnrollmentsAsync(int courseId);

    Task AddAsync(Course course);

    void Update(Course course);

    void Delete(Course course);

    Task SaveChangesAsync();
}