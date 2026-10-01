
using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _certificateRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseProgressService _courseProgressService;

    public CertificateService(
        ICertificateRepository certificateRepository,
        IEnrollmentRepository enrollmentRepository,
        ICourseProgressService courseProgressService)
    {
        _certificateRepository = certificateRepository;
        _enrollmentRepository = enrollmentRepository;
        _courseProgressService = courseProgressService;
    }

    public async Task<IEnumerable<CertificateDto>> GetAllAsync()
    {
        var certificates =
            await _certificateRepository.GetAllAsync();

        var result = new List<CertificateDto>();

        foreach (var certificate in certificates)
        {
            var enrollment =
                await _enrollmentRepository.GetByIdAsync(
                    certificate.EnrollmentId);

            if (enrollment == null)
                continue;

            result.Add(MapToDto(
                certificate,
                enrollment));
        }

        return result;
    }

    public async Task<CertificateDto?> GetByIdAsync(int id)
    {
        var certificate =
            await _certificateRepository.GetByIdAsync(id);

        if (certificate == null)
            return null;

        var enrollment =
            await _enrollmentRepository.GetByIdAsync(
                certificate.EnrollmentId);

        if (enrollment == null)
            return null;

        return MapToDto(
            certificate,
            enrollment);
    }

    public async Task<CertificateDto> CreateAsync(
        int enrollmentId,
        int userId)
    {
        var enrollment =
            await _enrollmentRepository.GetByIdAsync(
                enrollmentId);

        if (enrollment == null)
        {
            throw new KeyNotFoundException(
                "Enrollment not found.");
        }

        if (enrollment.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to issue a certificate for this enrollment.");
        }

        if (enrollment.Status != "Active")
        {
            throw new InvalidOperationException(
                "Certificate can only be issued for an active enrollment.");
        }

        var existingCertificate =
            await _certificateRepository
                .GetByEnrollmentIdAsync(
                    enrollmentId);

        if (existingCertificate != null)
        {
            throw new InvalidOperationException(
                "A certificate has already been issued for this enrollment.");
        }

        var progress =
            await _courseProgressService
                .GetProgressAsync(
                    enrollmentId);

        if (progress == null)
        {
            throw new InvalidOperationException(
                "Course progress could not be determined.");
        }

        if (!progress.IsCompleted)
        {
            throw new InvalidOperationException(
                "Certificate can only be issued after completing the course.");
        }

        var certificate = new Certificate
        {
            IssueDate = DateTime.UtcNow,
            CertificateUrl = string.Empty,
            EnrollmentId = enrollmentId
        };

        await _certificateRepository
            .AddAsync(certificate);

        await _certificateRepository
            .SaveChangesAsync();

        return MapToDto(
            certificate,
            enrollment);
    }

    public async Task DeleteAsync(int id)
    {
        var certificate =
            await _certificateRepository
                .GetByIdAsync(id);

        if (certificate == null)
        {
            throw new KeyNotFoundException(
                "Certificate not found.");
        }

        _certificateRepository.Delete(certificate);

        await _certificateRepository
            .SaveChangesAsync();
    }

    private static CertificateDto MapToDto(
        Certificate certificate,
        Enrollment enrollment)
    {
        return new CertificateDto
        {
            CertificateId =
                certificate.CertificateId,

            IssueDate =
                certificate.IssueDate,

            CertificateUrl =
                certificate.CertificateUrl,

            EnrollmentId =
                certificate.EnrollmentId,

            StudentName =
                enrollment.User?.Name ?? "Student",

            CourseTitle =
                enrollment.Course?.Title ?? "Course"
        };
    }
}
