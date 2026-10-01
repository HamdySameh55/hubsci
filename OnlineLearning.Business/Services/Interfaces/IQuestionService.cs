using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface IQuestionService
{
    Task<IEnumerable<QuestionDto>> GetAllAsync();

    Task<QuestionDto?> GetByIdAsync(int id);

    Task<QuestionDto> CreateAsync(CreateQuestionDto dto);

    Task UpdateAsync(int id, CreateQuestionDto dto);

    Task DeleteAsync(int id);
}