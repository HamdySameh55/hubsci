using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllAsync();

    Task<IEnumerable<CourseDto>> GetPublishedAsync();

    Task<IEnumerable<CourseDto>> GetByInstructorAsync(
        int instructorId);

    Task<CourseDto?> GetByIdAsync(
        int id);

    Task<CourseDto?> GetPublishedByIdAsync(
        int id);

    Task<CourseDto> CreateAsync(
        CreateCourseDto dto,
        int instructorId);

    Task UpdateAsync(
        int id,
        CreateCourseDto dto,
        int instructorId);

    Task DeleteAsync(
        int id,
        int instructorId);

    Task DeleteByAdminAsync(
        int id);

    Task PublishAsync(
        int id,
        int instructorId);
}