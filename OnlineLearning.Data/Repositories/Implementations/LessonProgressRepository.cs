using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Implementations;

public class LessonProgressRepository : ILessonProgressRepository
{
    private readonly ApplicationDbContext _context;

    public LessonProgressRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<LessonProgress>> GetAllAsync()
    {
        return await _context.LessonProgresses
            .ToListAsync();
    }

    public async Task<LessonProgress?> GetByIdAsync(int id)
    {
        return await _context.LessonProgresses
            .FindAsync(id);
    }

    public async Task<LessonProgress?> GetByIdForUserAsync(
        int id,
        int userId)
    {
        return await _context.LessonProgresses
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .Include(x => x.Lesson)
                .ThenInclude(x => x.Module)
                    .ThenInclude(x => x.Course)
            .FirstOrDefaultAsync(x =>
                x.ProgressId == id &&
                x.Enrollment.UserId == userId);
    }

    public async Task<IEnumerable<LessonProgress>> GetByEnrollmentIdAsync(
        int enrollmentId)
    {
        return await _context.LessonProgresses
            .Where(x => x.EnrollmentId == enrollmentId)
            .ToListAsync();
    }

    public async Task<IEnumerable<LessonProgress>> GetByUserIdAsync(
        int userId)
    {
        return await _context.LessonProgresses
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Course)
            .Include(x => x.Lesson)
                .ThenInclude(x => x.Module)
                    .ThenInclude(x => x.Course)
            .Where(x => x.Enrollment.UserId == userId)
            .ToListAsync();
    }

    public async Task AddAsync(LessonProgress progress)
    {
        await _context.LessonProgresses.AddAsync(progress);
    }

    public void Update(LessonProgress progress)
    {
        _context.LessonProgresses.Update(progress);
    }

    public void Delete(LessonProgress progress)
    {
        _context.LessonProgresses.Remove(progress);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}