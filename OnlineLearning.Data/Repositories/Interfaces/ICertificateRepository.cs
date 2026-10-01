using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Data.Repositories.Interfaces;

public interface ICertificateRepository
{
    Task<IEnumerable<Certificate>> GetAllAsync();

    Task<Certificate?> GetByIdAsync(int id);

    Task<Certificate?> GetByEnrollmentIdAsync(int enrollmentId);

    Task AddAsync(Certificate certificate);

    void Update(Certificate certificate);

    void Delete(Certificate certificate);

    Task SaveChangesAsync();
}