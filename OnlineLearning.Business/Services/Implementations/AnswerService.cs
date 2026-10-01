using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class AnswerService : IAnswerService
{
    private readonly IAnswerRepository _answerRepository;

    public AnswerService(IAnswerRepository answerRepository)
    {
        _answerRepository = answerRepository;
    }

    public async Task<IEnumerable<AnswerDto>> GetAllAsync()
    {
        var answers = await _answerRepository.GetAllAsync();

        return answers.Select(answer => new AnswerDto
        {
            AnswerId = answer.AnswerId,
            AnswerText = answer.AnswerText,
            IsCorrect = answer.IsCorrect,
            QuestionId = answer.QuestionId
        });
    }

    public async Task<AnswerDto?> GetByIdAsync(int id)
    {
        var answer = await _answerRepository.GetByIdAsync(id);

        if (answer == null)
            return null;

        return new AnswerDto
        {
            AnswerId = answer.AnswerId,
            AnswerText = answer.AnswerText,
            IsCorrect = answer.IsCorrect,
            QuestionId = answer.QuestionId
        };
    }

    public async Task<AnswerDto> CreateAsync(CreateAnswerDto dto)
    {
        var answer = new Answer
        {
            AnswerText = dto.AnswerText,
            IsCorrect = dto.IsCorrect,
            QuestionId = dto.QuestionId
        };

        await _answerRepository.AddAsync(answer);
        await _answerRepository.SaveChangesAsync();

        return new AnswerDto
        {
            AnswerId = answer.AnswerId,
            AnswerText = answer.AnswerText,
            IsCorrect = answer.IsCorrect,
            QuestionId = answer.QuestionId
        };
    }

    public async Task UpdateAsync(int id, CreateAnswerDto dto)
    {
        var answer = await _answerRepository.GetByIdAsync(id);

        if (answer == null)
            throw new KeyNotFoundException("Answer not found.");

        answer.AnswerText = dto.AnswerText;
        answer.IsCorrect = dto.IsCorrect;
        answer.QuestionId = dto.QuestionId;

        _answerRepository.Update(answer);

        await _answerRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var answer = await _answerRepository.GetByIdAsync(id);

        if (answer == null)
            throw new KeyNotFoundException("Answer not found.");

        _answerRepository.Delete(answer);

        await _answerRepository.SaveChangesAsync();
    }
}