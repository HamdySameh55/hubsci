using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class QuizAttemptService : IQuizAttemptService
{
    private readonly IQuizAttemptRepository _quizAttemptRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IQuizRepository _quizRepository;

    public QuizAttemptService(
        IQuizAttemptRepository quizAttemptRepository,
        IEnrollmentRepository enrollmentRepository,
        IQuizRepository quizRepository)
    {
        _quizAttemptRepository = quizAttemptRepository;
        _enrollmentRepository = enrollmentRepository;
        _quizRepository = quizRepository;
    }

    public async Task<IEnumerable<QuizAttemptDto>> GetAllAsync()
    {
        var attempts =
            await _quizAttemptRepository.GetAllAsync();

        return attempts.Select(MapToDto);
    }

    public async Task<QuizAttemptDto?> GetByIdAsync(int id)
    {
        var attempt =
            await _quizAttemptRepository.GetByIdAsync(id);

        if (attempt == null)
            return null;

        return MapToDto(attempt);
    }

    public async Task<QuizAttemptDto> CreateAsync(
        CreateQuizAttemptDto dto)
    {
        // 1. Check enrollment
        var enrollment =
            await _enrollmentRepository.GetByIdAsync(
                dto.EnrollmentId);

        if (enrollment == null)
        {
            throw new KeyNotFoundException(
                "Enrollment not found.");
        }

        // 2. Student must have an active enrollment
        if (enrollment.Status != "Active")
        {
            throw new InvalidOperationException(
                "Student must have an active enrollment to take the quiz.");
        }

        // 3. Get quiz with questions and answers
        var quiz =
            await _quizRepository.GetByIdWithQuestionsAsync(
                dto.QuizId);

        if (quiz == null)
        {
            throw new KeyNotFoundException(
                "Quiz not found.");
        }

        // 4. Check that the quiz belongs to the enrolled course
        if (quiz.Module.CourseId != enrollment.CourseId)
        {
            throw new InvalidOperationException(
                "This quiz does not belong to the enrolled course.");
        }

        // 5. Quiz must contain questions
        if (!quiz.Questions.Any())
        {
            throw new InvalidOperationException(
                "This quiz does not contain any questions.");
        }

        // 6. Check if student already passed this quiz
        var allAttempts =
            await _quizAttemptRepository.GetAllAsync();

        var hasPassedBefore =
            allAttempts.Any(attempt =>
                attempt.EnrollmentId == dto.EnrollmentId &&
                attempt.QuizId == dto.QuizId &&
                attempt.Passed);

        if (hasPassedBefore)
        {
            throw new InvalidOperationException(
                "You have already passed this quiz and cannot take it again.");
        }

        // 7. Get attempts from the last 24 hours
        var since =
            DateTime.UtcNow.AddHours(-24);

        var recentAttempts =
            (await _quizAttemptRepository.GetRecentAttemptsAsync(
                dto.EnrollmentId,
                dto.QuizId,
                since))
            .ToList();

        // 8. Quiz is locked after 3 failed attempts
        int failedAttempts =
            recentAttempts.Count(attempt => !attempt.Passed);

        if (failedAttempts >= 3)
        {
            throw new InvalidOperationException(
                "This quiz is locked because you failed 3 times within 24 hours.");
        }

        // 9. Student must answer every question
        if (dto.Answers.Count != quiz.Questions.Count)
        {
            throw new ArgumentException(
                "You must answer every question.");
        }

        // 10. Each question must appear exactly once
        var duplicatedQuestions =
            dto.Answers
                .GroupBy(answer => answer.QuestionId)
                .Any(group => group.Count() > 1);

        if (duplicatedQuestions)
        {
            throw new ArgumentException(
                "Each question must have exactly one selected answer.");
        }

        // 11. Validate that every question belongs to this quiz
        var questionIds =
            quiz.Questions
                .Select(question => question.QuestionId)
                .ToHashSet();

        if (dto.Answers.Any(
                answer => !questionIds.Contains(answer.QuestionId)))
        {
            throw new ArgumentException(
                "One or more questions do not belong to this quiz.");
        }

        // 12. Validate selected answers
        int correctAnswers = 0;

        foreach (var question in quiz.Questions)
        {
            var selection =
                dto.Answers.FirstOrDefault(
                    answer =>
                        answer.QuestionId ==
                        question.QuestionId);

            if (selection == null)
            {
                throw new ArgumentException(
                    "You must answer every question.");
            }

            var selectedAnswer =
                question.Answers.FirstOrDefault(
                    answer =>
                        answer.AnswerId ==
                        selection.SelectedAnswerId);

            if (selectedAnswer == null)
            {
                throw new ArgumentException(
                    "One or more selected answers do not belong to their questions.");
            }

            if (selectedAnswer.IsCorrect)
            {
                correctAnswers++;
            }
        }

        // 13. Calculate score
        int totalQuestions =
            quiz.Questions.Count;

        decimal score =
            (decimal)correctAnswers
            / totalQuestions
            * 100;

        // 14. Passing score = 60%
        bool passed =
            score >= 60;

        // 15. Calculate attempt number
        int attemptNumber =
            recentAttempts.Count + 1;

        // 16. Create attempt
        var attempt = new QuizAttempt
        {
            EnrollmentId = dto.EnrollmentId,
            QuizId = dto.QuizId,
            Score = score,
            AttemptDate = DateTime.UtcNow,
            Passed = passed,
            AttemptNumber = attemptNumber
        };

        await _quizAttemptRepository.AddAsync(attempt);

        await _quizAttemptRepository.SaveChangesAsync();

        return MapToDto(attempt);
    }

    public Task UpdateAsync(
        int id,
        CreateQuizAttemptDto dto)
    {
        throw new InvalidOperationException(
            "Quiz attempts cannot be modified after submission.");
    }

    public async Task DeleteAsync(int id)
    {
        var attempt =
            await _quizAttemptRepository.GetByIdAsync(id);

        if (attempt == null)
        {
            throw new KeyNotFoundException(
                "Quiz attempt not found.");
        }

        _quizAttemptRepository.Delete(attempt);

        await _quizAttemptRepository.SaveChangesAsync();
    }

    private static QuizAttemptDto MapToDto(
        QuizAttempt attempt)
    {
        return new QuizAttemptDto
        {
            QuizAttemptId = attempt.QuizAttemptId,
            EnrollmentId = attempt.EnrollmentId,
            QuizId = attempt.QuizId,
            Score = attempt.Score,
            AttemptDate = attempt.AttemptDate,
            Passed = attempt.Passed,
            AttemptNumber = attempt.AttemptNumber
        };
    }
}