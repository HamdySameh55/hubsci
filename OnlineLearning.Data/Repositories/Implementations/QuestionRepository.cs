using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Implementations;

public class QuestionRepository : IQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public QuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Question>> GetAllAsync()
    {
        return await _context.Questions.ToListAsync();
    }

    public async Task<Question?> GetByIdAsync(int id)
    {
        return await _context.Questions.FindAsync(id);
    }

    public async Task<Question?> GetByIdWithAnswersAsync(int id)
    {
        return await _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.QuestionId == id);
    }

    public async Task AddAsync(Question question)
    {
        await _context.Questions.AddAsync(question);
    }

    public void Update(Question question)
    {
        _context.Questions.Update(question);
    }

    public void Delete(Question question)
    {
        _context.Questions.Remove(question);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}