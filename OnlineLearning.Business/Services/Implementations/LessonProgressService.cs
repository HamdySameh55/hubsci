using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class LessonProgressService : ILessonProgressService
{
    private readonly ILessonProgressRepository _lessonProgressRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ILessonRepository _lessonRepository;

    public LessonProgressService(
        ILessonProgressRepository lessonProgressRepository,
        IEnrollmentRepository enrollmentRepository,
        ILessonRepository lessonRepository)
    {
        _lessonProgressRepository = lessonProgressRepository;
        _enrollmentRepository = enrollmentRepository;
        _lessonRepository = lessonRepository;
    }

    public async Task<IEnumerable<LessonProgressDto>> GetAllAsync()
    {
        var progressRecords =
            await _lessonProgressRepository.GetAllAsync();

        return progressRecords.Select(progress => new LessonProgressDto
        {
            ProgressId = progress.ProgressId,
            EnrollmentId = progress.EnrollmentId,
            LessonId = progress.LessonId,
            LessonTitle = progress.Lesson?.Title ?? string.Empty,
            CourseTitle = progress.Lesson?.Module?.Course?.Title
                          ?? progress.Enrollment?.Course?.Title
                          ?? string.Empty,
            IsCompleted = progress.IsCompleted,
            CompletedAt = progress.CompletedAt
        });
    }

    public async Task<bool> IsLessonCompletedAsync(
        int enrollmentId,
        int lessonId)
    {
        var progressRecords =
            await _lessonProgressRepository
                .GetByEnrollmentIdAsync(enrollmentId);

        return progressRecords.Any(x =>
            x.LessonId == lessonId &&
            x.IsCompleted);
    }

    public async Task<IEnumerable<LessonProgressDto>> GetByUserIdAsync(
        int userId)
    {
        var progressRecords =
            await _lessonProgressRepository
                .GetByUserIdAsync(userId);

        return progressRecords.Select(progress => new LessonProgressDto
        {
            ProgressId = progress.ProgressId,
            EnrollmentId = progress.EnrollmentId,
            LessonId = progress.LessonId,
            LessonTitle = progress.Lesson?.Title ?? string.Empty,
            CourseTitle = progress.Lesson?.Module?.Course?.Title
                          ?? progress.Enrollment?.Course?.Title
                          ?? string.Empty,
            IsCompleted = progress.IsCompleted,
            CompletedAt = progress.CompletedAt
        });
    }

    public async Task<LessonProgressDto?> GetByIdAsync(int id)
    {
        var progress =
            await _lessonProgressRepository.GetByIdAsync(id);

        if (progress == null)
            return null;

        return new LessonProgressDto
        {
            ProgressId = progress.ProgressId,
            EnrollmentId = progress.EnrollmentId,
            LessonId = progress.LessonId,
            LessonTitle = progress.Lesson?.Title ?? string.Empty,
            CourseTitle = progress.Lesson?.Module?.Course?.Title
                          ?? progress.Enrollment?.Course?.Title
                          ?? string.Empty,
            IsCompleted = progress.IsCompleted,
            CompletedAt = progress.CompletedAt
        };
    }

    public async Task<LessonProgressDto?> GetByIdForUserAsync(
        int id,
        int userId)
    {
        var progress =
            await _lessonProgressRepository
                .GetByIdForUserAsync(id, userId);

        if (progress == null)
            return null;

        return new LessonProgressDto
        {
            ProgressId = progress.ProgressId,
            EnrollmentId = progress.EnrollmentId,
            LessonId = progress.LessonId,
            LessonTitle = progress.Lesson?.Title ?? string.Empty,
            CourseTitle = progress.Lesson?.Module?.Course?.Title
                          ?? progress.Enrollment?.Course?.Title
                          ?? string.Empty,
            IsCompleted = progress.IsCompleted,
            CompletedAt = progress.CompletedAt
        };
    }

    public async Task<LessonProgressDto> CreateAsync(
        CreateLessonProgressDto dto,
        int userId)
    {
        // Check enrollment
        var enrollment =
            await _enrollmentRepository.GetByIdAsync(
                dto.EnrollmentId);

        if (enrollment == null)
            throw new KeyNotFoundException(
                "Enrollment not found.");

        // Make sure enrollment belongs to current user
        if (enrollment.UserId != userId)
            throw new UnauthorizedAccessException(
                "You are not allowed to update this enrollment.");

        // Student must have an active enrollment
        if (enrollment.Status != "Active")
            throw new InvalidOperationException(
                "You must have an active enrollment.");

        // Check lesson
        var lesson =
            await _lessonRepository.GetByIdAsync(
                dto.LessonId);

        if (lesson == null)
            throw new KeyNotFoundException(
                "Lesson not found.");

        // Get enrollment course
        var enrollmentCourseId = enrollment.CourseId;

        // Verify that the lesson belongs to the enrolled course
        var module =
            await _lessonRepository.GetModuleByLessonIdAsync(
                dto.LessonId);

        if (module == null)
            throw new KeyNotFoundException(
                "Module not found.");

        if (module.CourseId != enrollmentCourseId)
            throw new UnauthorizedAccessException(
                "This lesson does not belong to your course.");

        // Check if progress already exists
        var existingProgress =
            (await _lessonProgressRepository
                .GetByEnrollmentIdAsync(
                    dto.EnrollmentId))
            .FirstOrDefault(x =>
                x.LessonId == dto.LessonId);

        if (existingProgress != null)
        {
            throw new InvalidOperationException(
                "Progress for this lesson already exists.");
        }

        // Create progress
        var progress = new LessonProgress
        {
            EnrollmentId = dto.EnrollmentId,
            LessonId = dto.LessonId,
            IsCompleted = true,
            CompletedAt = DateTime.UtcNow
        };

        await _lessonProgressRepository.AddAsync(progress);

        await _lessonProgressRepository.SaveChangesAsync();

        return new LessonProgressDto
        {
            ProgressId = progress.ProgressId,
            EnrollmentId = progress.EnrollmentId,
            LessonId = progress.LessonId,
            LessonTitle = lesson.Title,
            CourseTitle = module.Course?.Title ?? string.Empty,
            IsCompleted = progress.IsCompleted,
            CompletedAt = progress.CompletedAt
        };
    }

    public async Task UpdateAsync(
        int id,
        CreateLessonProgressDto dto)
    {
        var progress =
            await _lessonProgressRepository.GetByIdAsync(id);

        if (progress == null)
            throw new KeyNotFoundException(
                "Lesson progress not found.");

        progress.EnrollmentId = dto.EnrollmentId;
        progress.LessonId = dto.LessonId;

        // Once completed, keep it completed
        progress.IsCompleted = true;

        if (progress.CompletedAt == null)
        {
            progress.CompletedAt = DateTime.UtcNow;
        }

        _lessonProgressRepository.Update(progress);

        await _lessonProgressRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var progress =
            await _lessonProgressRepository.GetByIdAsync(id);

        if (progress == null)
            throw new KeyNotFoundException(
                "Lesson progress not found.");

        _lessonProgressRepository.Delete(progress);

        await _lessonProgressRepository.SaveChangesAsync();
    }
}