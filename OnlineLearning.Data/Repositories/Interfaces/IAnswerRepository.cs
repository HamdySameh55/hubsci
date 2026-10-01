using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface IAnswerRepository
{
    Task<IEnumerable<Answer>> GetAllAsync();
    Task<Answer?> GetByIdAsync(int id);

    Task<bool> HasCorrectAnswerAsync(int questionId);

    Task AddAsync(Answer answer);
    void Update(Answer answer);
    void Delete(Answer answer);
    Task SaveChangesAsync();
}