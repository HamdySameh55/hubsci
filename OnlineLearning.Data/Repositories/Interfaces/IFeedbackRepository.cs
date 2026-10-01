using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface IFeedbackRepository
{
    Task<IEnumerable<Feedback>> GetAllAsync();

    Task<IEnumerable<Feedback>> GetByUserIdAsync(
        int userId);

    Task<Feedback?> GetByIdAsync(int id);

    Task<Feedback?> GetByUserAndCourseAsync(
        int userId,
        int courseId);

    Task AddAsync(Feedback feedback);

    void Update(Feedback feedback);

    void Delete(Feedback feedback);

    Task SaveChangesAsync();
}