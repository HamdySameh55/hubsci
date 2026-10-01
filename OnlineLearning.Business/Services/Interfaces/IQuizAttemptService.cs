using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface IQuizAttemptService
{
    Task<IEnumerable<QuizAttemptDto>> GetAllAsync();

    Task<QuizAttemptDto?> GetByIdAsync(int id);

    Task<QuizAttemptDto> CreateAsync(CreateQuizAttemptDto dto);

    Task UpdateAsync(int id, CreateQuizAttemptDto dto);

    Task DeleteAsync(int id);
}