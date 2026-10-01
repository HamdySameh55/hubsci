using Microsoft.EntityFrameworkCore;
using OnlineLearning.Data.Context;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Implementations;

public class CertificateRepository : ICertificateRepository
{
    private readonly ApplicationDbContext _context;

    public CertificateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Certificate>> GetAllAsync()
    {
        return await _context.Certificates.ToListAsync();
    }

    public async Task<Certificate?> GetByIdAsync(int id)
    {
        return await _context.Certificates.FindAsync(id);
    }

    public async Task<Certificate?> GetByEnrollmentIdAsync(
        int enrollmentId)
    {
        return await _context.Certificates
            .FirstOrDefaultAsync(x =>
                x.EnrollmentId == enrollmentId);
    }

    public async Task AddAsync(Certificate certificate)
    {
        await _context.Certificates.AddAsync(certificate);
    }

    public void Update(Certificate certificate)
    {
        _context.Certificates.Update(certificate);
    }

    public void Delete(Certificate certificate)
    {
        _context.Certificates.Remove(certificate);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}