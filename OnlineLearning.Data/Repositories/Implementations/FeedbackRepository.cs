using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Implementations;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly ApplicationDbContext _context;

    public FeedbackRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Feedback>> GetAllAsync()
    {
        return await _context.Feedbacks
            .ToListAsync();
    }

    public async Task<IEnumerable<Feedback>> GetByUserIdAsync(
        int userId)
    {
        return await _context.Feedbacks
            .Where(feedback =>
                feedback.UserId == userId)
            .ToListAsync();
    }

    public async Task<Feedback?> GetByIdAsync(int id)
    {
        return await _context.Feedbacks
            .FindAsync(id);
    }

    public async Task<Feedback?> GetByUserAndCourseAsync(
        int userId,
        int courseId)
    {
        return await _context.Feedbacks
            .FirstOrDefaultAsync(feedback =>
                feedback.UserId == userId &&
                feedback.CourseId == courseId);
    }

    public async Task AddAsync(Feedback feedback)
    {
        await _context.Feedbacks
            .AddAsync(feedback);
    }

    public void Update(Feedback feedback)
    {
        _context.Feedbacks
            .Update(feedback);
    }

    public void Delete(Feedback feedback)
    {
        _context.Feedbacks
            .Remove(feedback);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}