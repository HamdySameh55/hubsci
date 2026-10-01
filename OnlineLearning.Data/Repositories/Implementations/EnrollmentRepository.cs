using Microsoft.EntityFrameworkCore;

using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Implementations;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly ApplicationDbContext _context;

    public EnrollmentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Enrollment>> GetAllAsync()
    {
        return await _context.Enrollments
            .Include(x => x.Course)
            .Include(x => x.User)
            .ToListAsync();
    }

    public async Task<IEnumerable<Enrollment>> GetByUserIdAsync(int userId)
    {
        return await _context.Enrollments
            .Include(x => x.Course)
            .Include(x => x.User)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<Enrollment?> GetByIdAsync(int id)
    {
        return await _context.Enrollments
            .Include(x => x.Course)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.EnrollmentId == id);
    }

    public async Task AddAsync(Enrollment enrollment)
    {
        await _context.Enrollments.AddAsync(enrollment);
    }

    public async Task<Enrollment?> GetByUserAndCourseAsync(
        int userId,
        int courseId)
    {
        return await _context.Enrollments
            .Include(x => x.Course)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.CourseId == courseId);
    }

    public async Task<bool> HasActiveEnrollmentAsync(
        int userId,
        int courseId)
    {
        return await _context.Enrollments
            .AnyAsync(x =>
                x.UserId == userId &&
                x.CourseId == courseId &&
                x.Status == "Active");
    }

    public async Task<Enrollment?> GetActiveEnrollmentAsync(
        int userId,
        int courseId)
    {
        return await _context.Enrollments
            .Include(x => x.Course)
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.CourseId == courseId &&
                x.Status == "Active");
    }

    public void Update(Enrollment enrollment)
    {
        _context.Enrollments.Update(enrollment);
    }

    public void Delete(Enrollment enrollment)
    {
        _context.Enrollments.Remove(enrollment);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}