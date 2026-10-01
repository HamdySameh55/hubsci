using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _questionRepository;

    public QuestionService(
        IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<IEnumerable<QuestionDto>> GetAllAsync()
    {
        var questions =
            await _questionRepository.GetAllAsync();

        return questions.Select(question => new QuestionDto
        {
            QuestionId = question.QuestionId,
            QuestionText = question.QuestionText,
            QuizId = question.QuizId
        });
    }

    public async Task<QuestionDto?> GetByIdAsync(int id)
    {
        var question =
            await _questionRepository
                .GetByIdWithAnswersAsync(id);

        if (question == null)
            return null;

        return new QuestionDto
        {
            QuestionId = question.QuestionId,
            QuestionText = question.QuestionText,
            QuizId = question.QuizId,

            Answers = question.Answers
                .Select(answer => new QuizAnswerDto
                {
                    AnswerId = answer.AnswerId,
                    AnswerText = answer.AnswerText
                })
                .ToList()
        };
    }

    public async Task<QuestionDto> CreateAsync(
        CreateQuestionDto dto)
    {
        var question = new Question
        {
            QuestionText = dto.QuestionText,
            QuizId = dto.QuizId
        };

        await _questionRepository.AddAsync(question);

        await _questionRepository.SaveChangesAsync();

        return new QuestionDto
        {
            QuestionId = question.QuestionId,
            QuestionText = question.QuestionText,
            QuizId = question.QuizId
        };
    }

    public async Task UpdateAsync(
        int id,
        CreateQuestionDto dto)
    {
        var question =
            await _questionRepository.GetByIdAsync(id);

        if (question == null)
            throw new KeyNotFoundException(
                "Question not found.");

        question.QuestionText = dto.QuestionText;
        question.QuizId = dto.QuizId;

        _questionRepository.Update(question);

        await _questionRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var question =
            await _questionRepository.GetByIdAsync(id);

        if (question == null)
            throw new KeyNotFoundException(
                "Question not found.");

        _questionRepository.Delete(question);

        await _questionRepository.SaveChangesAsync();
    }
}