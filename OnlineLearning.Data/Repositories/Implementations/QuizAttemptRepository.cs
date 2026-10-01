using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Implementations;

public class QuizAttemptRepository : IQuizAttemptRepository
{
    private readonly ApplicationDbContext _context;

    public QuizAttemptRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<QuizAttempt>> GetAllAsync()
    {
        return await _context.QuizAttempts.ToListAsync();
    }

    public async Task<QuizAttempt?> GetByIdAsync(int id)
    {
        return await _context.QuizAttempts.FindAsync(id);
    }

    public async Task AddAsync(QuizAttempt quizAttempt)
    {
        await _context.QuizAttempts.AddAsync(quizAttempt);
    }

    public void Update(QuizAttempt quizAttempt)
    {
        _context.QuizAttempts.Update(quizAttempt);
    }

    public void Delete(QuizAttempt quizAttempt)
    {
        _context.QuizAttempts.Remove(quizAttempt);
    }
    public async Task<IEnumerable<QuizAttempt>> GetRecentAttemptsAsync(
    int enrollmentId,
    int quizId,
    DateTime since)
{
    return await _context.QuizAttempts
        .Where(x =>
            x.EnrollmentId == enrollmentId &&
            x.QuizId == quizId &&
            x.AttemptDate >= since)
        .OrderBy(x => x.AttemptDate)
        .ToListAsync();
}

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}