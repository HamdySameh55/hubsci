using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class ModuleService : IModuleService
{
    private readonly IModuleRepository _moduleRepository;
    private readonly ICourseRepository _courseRepository;

    public ModuleService(
        IModuleRepository moduleRepository,
        ICourseRepository courseRepository)
    {
        _moduleRepository = moduleRepository;
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<ModuleDto>> GetAllAsync()
    {
        var modules = await _moduleRepository.GetAllAsync();

        return modules.Select(module => new ModuleDto
        {
            ModuleId = module.ModuleId,
            Title = module.Title,
            Description = module.Description,
            OrderNumber = module.OrderNumber,
            CourseId = module.CourseId
        });
    }

    public async Task<IEnumerable<ModuleDto>> GetByCourseIdAsync(
        int courseId)
    {
        var modules = await _moduleRepository.GetAllAsync();

        return modules
            .Where(module => module.CourseId == courseId)
            .OrderBy(module => module.OrderNumber)
            .Select(module => new ModuleDto
            {
                ModuleId = module.ModuleId,
                Title = module.Title,
                Description = module.Description,
                OrderNumber = module.OrderNumber,
                CourseId = module.CourseId
            });
    }

    public async Task<ModuleDto?> GetByIdAsync(int id)
    {
        var module = await _moduleRepository.GetByIdAsync(id);

        if (module == null)
            return null;

        return new ModuleDto
        {
            ModuleId = module.ModuleId,
            Title = module.Title,
            Description = module.Description,
            OrderNumber = module.OrderNumber,
            CourseId = module.CourseId
        };
    }

    public async Task<ModuleDto> CreateAsync(
        CreateModuleDto dto,
        int courseId,
        int instructorId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);

        if (course == null)
            throw new KeyNotFoundException("Course not found.");

        if (course.InstructorId != instructorId)
            throw new UnauthorizedAccessException(
                "You are not allowed to add modules to this course.");

        var existingModules =
            await _moduleRepository.GetAllAsync();

        var orderExists = existingModules.Any(module =>
            module.CourseId == courseId &&
            module.OrderNumber == dto.OrderNumber);

        if (orderExists)
            throw new InvalidOperationException(
                "This order number is already used in this course.");

        var module = new Module
        {
            Title = dto.Title,
            Description = dto.Description,
            OrderNumber = dto.OrderNumber,
            CourseId = courseId
        };

        await _moduleRepository.AddAsync(module);

        await _moduleRepository.SaveChangesAsync();

        return new ModuleDto
        {
            ModuleId = module.ModuleId,
            Title = module.Title,
            Description = module.Description,
            OrderNumber = module.OrderNumber,
            CourseId = module.CourseId
        };
    }

    public async Task UpdateAsync(
        int id,
        CreateModuleDto dto,
        int instructorId)
    {
        var module = await _moduleRepository.GetByIdAsync(id);

        if (module == null)
            throw new KeyNotFoundException("Module not found.");

        var course =
            await _courseRepository.GetByIdAsync(module.CourseId);

        if (course == null)
            throw new KeyNotFoundException("Course not found.");

        if (course.InstructorId != instructorId)
            throw new UnauthorizedAccessException(
                "You are not allowed to modify this module.");

        var existingModules =
            await _moduleRepository.GetAllAsync();

        var orderExists = existingModules.Any(existingModule =>
            existingModule.ModuleId != id &&
            existingModule.CourseId == module.CourseId &&
            existingModule.OrderNumber == dto.OrderNumber);

        if (orderExists)
            throw new InvalidOperationException(
                "This order number is already used in this course.");

        module.Title = dto.Title;
        module.Description = dto.Description;
        module.OrderNumber = dto.OrderNumber;

        _moduleRepository.Update(module);

        await _moduleRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        int id,
        int instructorId)
    {
        var module = await _moduleRepository.GetByIdAsync(id);

        if (module == null)
            throw new KeyNotFoundException("Module not found.");

        var course =
            await _courseRepository.GetByIdAsync(module.CourseId);

        if (course == null)
            throw new KeyNotFoundException("Course not found.");

        if (course.InstructorId != instructorId)
            throw new UnauthorizedAccessException(
                "You are not allowed to delete this module.");

        if (await _courseRepository.HasEnrollmentsAsync(course.CourseId))
            throw new InvalidOperationException(
                "This module belongs to a course with enrolled students and cannot be deleted.");

        _moduleRepository.Delete(module);

        await _moduleRepository.SaveChangesAsync();
    }
}