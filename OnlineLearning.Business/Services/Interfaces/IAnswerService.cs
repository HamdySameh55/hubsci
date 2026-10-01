using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface IAnswerService
{
    Task<IEnumerable<AnswerDto>> GetAllAsync();

    Task<AnswerDto?> GetByIdAsync(int id);

    Task<AnswerDto> CreateAsync(CreateAnswerDto dto);

    Task UpdateAsync(int id, CreateAnswerDto dto);

    Task DeleteAsync(int id);
}