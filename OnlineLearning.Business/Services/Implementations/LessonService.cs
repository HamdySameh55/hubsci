using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class LessonService : ILessonService
{
    private readonly ILessonRepository _lessonRepository;
    private readonly IModuleRepository _moduleRepository;
    private readonly ICourseRepository _courseRepository;

    public LessonService(
        ILessonRepository lessonRepository,
        IModuleRepository moduleRepository,
        ICourseRepository courseRepository)
    {
        _lessonRepository = lessonRepository;
        _moduleRepository = moduleRepository;
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<LessonDto>> GetAllAsync()
    {
        var lessons =
            await _lessonRepository.GetAllAsync();

        var lessonDtos = new List<LessonDto>();

        foreach (var lesson in lessons)
        {
            var module =
                await _moduleRepository.GetByIdAsync(
                    lesson.ModuleId);

            if (module == null)
                continue;

            lessonDtos.Add(new LessonDto
            {
                LessonId = lesson.LessonId,
                Title = lesson.Title,
                Description = lesson.Description,
                ContentType = lesson.ContentType,
                ContentUrl = lesson.ContentUrl,
                ModuleId = lesson.ModuleId,
                CourseId = module.CourseId
            });
        }

        return lessonDtos;
    }

    public async Task<LessonDto?> GetByIdAsync(int id)
    {
        var lesson =
            await _lessonRepository.GetByIdAsync(id);

        if (lesson == null)
            return null;

        var module =
            await _moduleRepository.GetByIdAsync(
                lesson.ModuleId);

        if (module == null)
            throw new KeyNotFoundException(
                "Module not found.");

        return new LessonDto
        {
            LessonId = lesson.LessonId,
            Title = lesson.Title,
            Description = lesson.Description,
            ContentType = lesson.ContentType,
            ContentUrl = lesson.ContentUrl,
            ModuleId = lesson.ModuleId,
            CourseId = module.CourseId
        };
    }

    public async Task<LessonDto> CreateAsync(
        CreateLessonDto dto,
        int instructorId)
    {
        var module =
            await _moduleRepository.GetByIdAsync(
                dto.ModuleId);

        if (module == null)
            throw new KeyNotFoundException(
                "Module not found.");

        var course =
            await _courseRepository.GetByIdAsync(
                module.CourseId);

        if (course == null)
            throw new KeyNotFoundException(
                "Course not found.");

        if (course.InstructorId != instructorId)
            throw new UnauthorizedAccessException(
                "You are not allowed to add lessons to this module.");

        var lesson = new Lesson
        {
            Title = dto.Title,
            Description = dto.Description,
            ContentType = dto.ContentType,
            ContentUrl = dto.ContentUrl,
            ModuleId = dto.ModuleId
        };

        await _lessonRepository.AddAsync(lesson);

        await _lessonRepository.SaveChangesAsync();

        return new LessonDto
        {
            LessonId = lesson.LessonId,
            Title = lesson.Title,
            Description = lesson.Description,
            ContentType = lesson.ContentType,
            ContentUrl = lesson.ContentUrl,
            ModuleId = lesson.ModuleId,
            CourseId = module.CourseId
        };
    }

    public async Task UpdateAsync(
        int id,
        CreateLessonDto dto,
        int instructorId)
    {
        var lesson =
            await _lessonRepository.GetByIdAsync(id);

        if (lesson == null)
            throw new KeyNotFoundException(
                "Lesson not found.");

        var module =
            await _moduleRepository.GetByIdAsync(
                lesson.ModuleId);

        if (module == null)
            throw new KeyNotFoundException(
                "Module not found.");

        var course =
            await _courseRepository.GetByIdAsync(
                module.CourseId);

        if (course == null)
            throw new KeyNotFoundException(
                "Course not found.");

        if (course.InstructorId != instructorId)
            throw new UnauthorizedAccessException(
                "You are not allowed to modify this lesson.");

        lesson.Title = dto.Title;
        lesson.Description = dto.Description;
        lesson.ContentType = dto.ContentType;
        lesson.ContentUrl = dto.ContentUrl;

        _lessonRepository.Update(lesson);

        await _lessonRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        int id,
        int instructorId)
    {
        var lesson =
            await _lessonRepository.GetByIdAsync(id);

        if (lesson == null)
            throw new KeyNotFoundException(
                "Lesson not found.");

        var module =
            await _moduleRepository.GetByIdAsync(
                lesson.ModuleId);

        if (module == null)
            throw new KeyNotFoundException(
                "Module not found.");

        var course =
            await _courseRepository.GetByIdAsync(
                module.CourseId);

        if (course == null)
            throw new KeyNotFoundException(
                "Course not found.");

        if (course.InstructorId != instructorId)
            throw new UnauthorizedAccessException(
                "You are not allowed to delete this lesson.");

        // Prevent deleting a lesson that has student progress
        var hasProgress =
            await _lessonRepository.HasProgressAsync(id);

        if (hasProgress)
        {
            throw new InvalidOperationException(
                "Cannot delete this lesson because students have already made progress on it.");
        }

        _lessonRepository.Delete(lesson);

        await _lessonRepository.SaveChangesAsync();
    }

    public async Task DeleteByAdminAsync(int id)
    {
        var lesson =
            await _lessonRepository.GetByIdAsync(id);

        if (lesson == null)
            throw new KeyNotFoundException(
                "Lesson not found.");

        _lessonRepository.Delete(lesson);

        await _lessonRepository.SaveChangesAsync();
    }
}