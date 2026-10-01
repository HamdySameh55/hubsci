using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Implementations;

public class QuizRepository : IQuizRepository
{
    private readonly ApplicationDbContext _context;

    public QuizRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Quiz>> GetAllAsync()
    {
        return await _context.Quizzes.ToListAsync();
    }

    public async Task<Quiz?> GetByIdAsync(int id)
    {
        return await _context.Quizzes.FindAsync(id);
    }

    public async Task<Quiz?> GetByIdWithQuestionsAsync(int id)
    {
        return await _context.Quizzes
            .Include(q => q.Module)
            .Include(q => q.Questions)
            .ThenInclude(question => question.Answers)
            .FirstOrDefaultAsync(q => q.QuizId == id);
    }

    public async Task<IEnumerable<Quiz>> GetByCourseIdAsync(int courseId)
    {
        return await _context.Quizzes
            .Where(q => q.Module.CourseId == courseId)
            .ToListAsync();
    }

    public async Task<bool> HasAttemptsAsync(int quizId)
    {
        return await _context.QuizAttempts
            .AnyAsync(attempt => attempt.QuizId == quizId);
    }

    public async Task AddAsync(Quiz quiz)
    {
        await _context.Quizzes.AddAsync(quiz);
    }

    public void Update(Quiz quiz)
    {
        _context.Quizzes.Update(quiz);
    }

    public void Delete(Quiz quiz)
    {
        _context.Quizzes.Remove(quiz);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}