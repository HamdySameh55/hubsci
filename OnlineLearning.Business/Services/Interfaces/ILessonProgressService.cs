using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface ILessonProgressService
{
    Task<IEnumerable<LessonProgressDto>> GetAllAsync();

    Task<IEnumerable<LessonProgressDto>> GetByUserIdAsync(
        int userId);

    Task<LessonProgressDto?> GetByIdAsync(int id);

    Task<LessonProgressDto?> GetByIdForUserAsync(
        int id,
        int userId);

    Task<LessonProgressDto> CreateAsync(
        CreateLessonProgressDto dto,
        int userId);

    Task<bool> IsLessonCompletedAsync(
        int enrollmentId,
        int lessonId);

    Task UpdateAsync(
        int id,
        CreateLessonProgressDto dto);

    Task DeleteAsync(int id);
}