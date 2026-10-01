using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface IQuizAttemptRepository
{
    Task<IEnumerable<QuizAttempt>> GetAllAsync();

    Task<QuizAttempt?> GetByIdAsync(int id);

    Task<IEnumerable<QuizAttempt>> GetRecentAttemptsAsync(
        int enrollmentId,
        int quizId,
        DateTime since);

    Task AddAsync(QuizAttempt quizAttempt);

    void Update(QuizAttempt quizAttempt);

    void Delete(QuizAttempt quizAttempt);

    Task SaveChangesAsync();
}