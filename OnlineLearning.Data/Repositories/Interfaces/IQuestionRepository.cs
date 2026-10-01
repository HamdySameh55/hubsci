using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface IQuestionRepository
{
    Task<IEnumerable<Question>> GetAllAsync();

    Task<Question?> GetByIdAsync(int id);

    Task<Question?> GetByIdWithAnswersAsync(int id);

    Task AddAsync(Question question);

    void Update(Question question);

    void Delete(Question question);

    Task SaveChangesAsync();
}