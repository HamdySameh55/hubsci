using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface ICertificateService
{
    Task<IEnumerable<CertificateDto>> GetAllAsync();

    Task<CertificateDto?> GetByIdAsync(int id);

    Task<CertificateDto> CreateAsync(
        int enrollmentId,
        int userId);

    Task DeleteAsync(int id);
}