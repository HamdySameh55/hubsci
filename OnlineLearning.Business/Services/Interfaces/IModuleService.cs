using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface IModuleService
{
    Task<IEnumerable<ModuleDto>> GetAllAsync();

    Task<IEnumerable<ModuleDto>> GetByCourseIdAsync(
        int courseId);

    Task<ModuleDto?> GetByIdAsync(
        int id);

    Task<ModuleDto> CreateAsync(
        CreateModuleDto dto,
        int courseId,
        int instructorId);

    Task UpdateAsync(
        int id,
        CreateModuleDto dto,
        int instructorId);

    Task DeleteAsync(
        int id,
        int instructorId);
}