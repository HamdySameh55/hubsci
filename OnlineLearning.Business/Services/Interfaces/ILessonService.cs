using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface ILessonService
{
    Task<IEnumerable<LessonDto>> GetAllAsync();

    Task<LessonDto?> GetByIdAsync(int id);

    Task<LessonDto> CreateAsync(
        CreateLessonDto dto,
        int instructorId);

    Task UpdateAsync(
        int id,
        CreateLessonDto dto,
        int instructorId);

    Task DeleteAsync(
        int id,
        int instructorId);

    Task DeleteByAdminAsync(int id);
}