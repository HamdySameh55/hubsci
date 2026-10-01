using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface IQuizRepository
{
    Task<IEnumerable<Quiz>> GetAllAsync();

    Task<Quiz?> GetByIdAsync(int id);

    Task<Quiz?> GetByIdWithQuestionsAsync(int id);

    Task<IEnumerable<Quiz>> GetByCourseIdAsync(int courseId);

    Task<bool> HasAttemptsAsync(int quizId);

    Task AddAsync(Quiz quiz);

    void Update(Quiz quiz);

    void Delete(Quiz quiz);

    Task SaveChangesAsync();
}