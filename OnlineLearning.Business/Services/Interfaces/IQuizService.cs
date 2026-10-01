using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface IQuizService
{
    Task<IEnumerable<QuizDto>> GetAllAsync();

    Task<QuizDto?> GetByIdAsync(int id);

    Task<QuizDto?> GetByIdWithQuestionsAsync(int id);

    Task<QuizDto> CreateAsync(CreateQuizDto dto);

    Task UpdateAsync(int id, CreateQuizDto dto);

    Task DeleteAsync(int id);
}