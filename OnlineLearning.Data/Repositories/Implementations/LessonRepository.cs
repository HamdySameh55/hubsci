
using Microsoft.EntityFrameworkCore;

using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Implementations;

public class LessonRepository : ILessonRepository
{
    private readonly ApplicationDbContext _context;

    public LessonRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }
public async Task<bool> HasProgressAsync(int lessonId)
{
    return await _context.LessonProgresses
        .AnyAsync(progress => progress.LessonId == lessonId);
}
    public async Task<IEnumerable<Lesson>> GetAllAsync()
    {
        return await _context.Lessons
            .ToListAsync();
    }

    public async Task<Lesson?> GetByIdAsync(int id)
    {
        return await _context.Lessons
            .FindAsync(id);
    }

    public async Task<Module?> GetModuleByLessonIdAsync(
        int lessonId)
    {
        return await _context.Modules
            .FirstOrDefaultAsync(x =>
                x.Lessons.Any(l =>
                    l.LessonId == lessonId));
    }

    public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(
        int courseId)
    {
        return await _context.Lessons
            .Where(x =>
                x.Module.CourseId == courseId)
            .ToListAsync();
    }

    public async Task AddAsync(Lesson lesson)
    {
        await _context.Lessons.AddAsync(lesson);
    }

    public void Update(Lesson lesson)
    {
        _context.Lessons.Update(lesson);
    }

    public void Delete(Lesson lesson)
    {
        _context.Lessons.Remove(lesson);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
