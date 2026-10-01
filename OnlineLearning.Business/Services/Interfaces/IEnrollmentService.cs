using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface IEnrollmentService
{
    Task<IEnumerable<EnrollmentDto>> GetAllAsync();

    Task<IEnumerable<EnrollmentDto>> GetByUserIdAsync(int userId);

    Task<EnrollmentDto?> GetByIdAsync(int id);

    Task<EnrollmentDto> CreateAsync(
        CreateEnrollmentDto dto,
        int userId);

    Task<bool> HasActiveEnrollmentAsync(
        int userId,
        int courseId);

    Task<EnrollmentDto?> GetActiveEnrollmentAsync(
        int userId,
        int courseId);

    Task DeleteAsync(
        int id,
        int userId);
}