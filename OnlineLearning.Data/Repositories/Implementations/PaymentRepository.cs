using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Implementations;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;

    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        return await _context.Payments.ToListAsync();
    }

    public async Task<Payment?> GetByIdAsync(int id)
    {
        return await _context.Payments.FindAsync(id);
    }

    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
    }

    public void Update(Payment payment)
    {
        _context.Payments.Update(payment);
    }

    public void Delete(Payment payment)
    {
        _context.Payments.Remove(payment);
    }
public async Task<Payment?> GetByIdWithEnrollmentAsync(int id)
    {
        return await _context.Payments
            .Include(payment => payment.Enrollment)
                .ThenInclude(enrollment => enrollment.User)
            .Include(payment => payment.Enrollment)
                .ThenInclude(enrollment => enrollment.Course)
            .FirstOrDefaultAsync(payment => payment.PaymentId == id);
    }

    public async Task<IEnumerable<Payment>> GetPendingForInstructorAsync(
        int instructorId)
    {
        return await _context.Payments
            .Include(payment => payment.Enrollment)
                .ThenInclude(enrollment => enrollment.User)
            .Include(payment => payment.Enrollment)
                .ThenInclude(enrollment => enrollment.Course)
            .Where(payment =>
                payment.PaymentStatus == "Pending" &&
                payment.Enrollment.Course.InstructorId == instructorId)
            .OrderByDescending(payment => payment.PaymentDate)
            .ToListAsync();
    }

public async Task<Payment?> GetByEnrollmentIdAsync(int enrollmentId)
{
    return await _context.Payments
        .FirstOrDefaultAsync(x => x.EnrollmentId == enrollmentId);
}

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}