
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);

    Task<User?> GetByEmailAsync(string email);

    Task<bool> HasEnrollmentsAsync(int userId);

    Task<bool> HasCoursesAsync(int userId);

    Task<bool> HasFeedbacksAsync(int userId);

    Task AddAsync(User user);

    void Update(User user);

    void Delete(User user);

    Task SaveChangesAsync();
}
