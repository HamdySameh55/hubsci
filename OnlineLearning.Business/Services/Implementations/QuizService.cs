using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class QuizService : IQuizService
{
    private readonly IQuizRepository _quizRepository;

    public QuizService(IQuizRepository quizRepository)
    {
        _quizRepository = quizRepository;
    }

    public async Task<IEnumerable<QuizDto>> GetAllAsync()
    {
        var quizzes =
            await _quizRepository.GetAllAsync();

        return quizzes.Select(quiz => new QuizDto
        {
            QuizId = quiz.QuizId,
            Title = quiz.Title,
            Description = quiz.Description,
            ModuleId = quiz.ModuleId
        });
    }

    public async Task<QuizDto?> GetByIdAsync(int id)
    {
        var quiz =
            await _quizRepository.GetByIdAsync(id);

        if (quiz == null)
            return null;

        return new QuizDto
        {
            QuizId = quiz.QuizId,
            Title = quiz.Title,
            Description = quiz.Description,
            ModuleId = quiz.ModuleId
        };
    }

    public async Task<QuizDto?> GetByIdWithQuestionsAsync(int id)
    {
        var quiz =
            await _quizRepository.GetByIdWithQuestionsAsync(id);

        if (quiz == null)
            return null;

        return new QuizDto
        {
            QuizId = quiz.QuizId,
            Title = quiz.Title,
            Description = quiz.Description,
            ModuleId = quiz.ModuleId,
            CourseId = quiz.Module.CourseId,

            Questions = quiz.Questions
                .Select(question => new QuestionDto
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
                })
                .ToList()
        };
    }

    public async Task<QuizDto> CreateAsync(
        CreateQuizDto dto)
    {
        var quiz = new Quiz
        {
            Title = dto.Title,
            Description = dto.Description,
            ModuleId = dto.ModuleId
        };

        await _quizRepository.AddAsync(quiz);

        await _quizRepository.SaveChangesAsync();

        return new QuizDto
        {
            QuizId = quiz.QuizId,
            Title = quiz.Title,
            Description = quiz.Description,
            ModuleId = quiz.ModuleId
        };
    }

    public async Task UpdateAsync(
        int id,
        CreateQuizDto dto)
    {
        var quiz =
            await _quizRepository.GetByIdAsync(id);

        if (quiz == null)
            throw new KeyNotFoundException(
                "Quiz not found.");

        quiz.Title = dto.Title;
        quiz.Description = dto.Description;
        quiz.ModuleId = dto.ModuleId;

        _quizRepository.Update(quiz);

        await _quizRepository.SaveChangesAsync();
    }

public async Task DeleteAsync(int id)
{
    var quiz =
        await _quizRepository.GetByIdAsync(id);

    if (quiz == null)
        throw new KeyNotFoundException(
            "Quiz not found.");

    var hasAttempts =
        await _quizRepository.HasAttemptsAsync(id);

    if (hasAttempts)
    {
        throw new InvalidOperationException(
            "Cannot delete this quiz because students have already attempted it.");
    }

    _quizRepository.Delete(quiz);

    await _quizRepository.SaveChangesAsync();
}
}