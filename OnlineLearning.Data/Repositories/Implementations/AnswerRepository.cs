using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Implementations;

public class AnswerRepository : IAnswerRepository
{
    private readonly ApplicationDbContext _context;

    public AnswerRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Answer>> GetAllAsync()
    {
        return await _context.Answers.ToListAsync();
    }

    public async Task<Answer?> GetByIdAsync(int id)
    {
        return await _context.Answers.FindAsync(id);
    }

    public async Task<bool> HasCorrectAnswerAsync(int questionId)
    {
        return await _context.Answers
            .AnyAsync(answer =>
                answer.QuestionId == questionId &&
                answer.IsCorrect);
    }

    public async Task AddAsync(Answer answer)
    {
        await _context.Answers.AddAsync(answer);
    }

    public void Update(Answer answer)
    {
        _context.Answers.Update(answer);
    }

    public void Delete(Answer answer)
    {
        _context.Answers.Remove(answer);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}