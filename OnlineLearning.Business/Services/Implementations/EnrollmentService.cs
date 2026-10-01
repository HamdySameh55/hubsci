using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseRepository _courseRepository;

    public EnrollmentService(
        IEnrollmentRepository enrollmentRepository,
        ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<EnrollmentDto>> GetAllAsync()
    {
        var enrollments =
            await _enrollmentRepository.GetAllAsync();

        return enrollments.Select(enrollment => new EnrollmentDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId
        });
    }

    public async Task<IEnumerable<EnrollmentDto>> GetByUserIdAsync(
        int userId)
    {
        var enrollments =
            await _enrollmentRepository.GetByUserIdAsync(userId);

        return enrollments.Select(enrollment => new EnrollmentDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId
        });
    }

    public async Task<EnrollmentDto?> GetByIdAsync(int id)
    {
        var enrollment =
            await _enrollmentRepository.GetByIdAsync(id);

        if (enrollment == null)
            return null;

        return new EnrollmentDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId
        };
    }

    public async Task<EnrollmentDto> CreateAsync(
        CreateEnrollmentDto dto,
        int userId)
    {
        var course =
            await _courseRepository.GetByIdAsync(dto.CourseId);

        if (course == null)
            throw new KeyNotFoundException(
                "Course not found.");

        if (course.Status != "Published")
        {
            throw new InvalidOperationException(
                "You can enroll only in published courses.");
        }

        var existingEnrollment =
            await _enrollmentRepository.GetByUserAndCourseAsync(
                userId,
                dto.CourseId);

        if (existingEnrollment != null)
        {
            throw new InvalidOperationException(
                "Student is already enrolled in this course.");
        }

        var enrollment = new Enrollment
        {
            EnrollmentDate = DateTime.UtcNow,
            Status = course.Price > 0 ? "Pending" : "Active",
            UserId = userId,
            CourseId = dto.CourseId
        };

        await _enrollmentRepository.AddAsync(enrollment);

        await _enrollmentRepository.SaveChangesAsync();

        return new EnrollmentDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId
        };
    }

    public async Task<bool> HasActiveEnrollmentAsync(
        int userId,
        int courseId)
    {
        return await _enrollmentRepository
            .HasActiveEnrollmentAsync(
                userId,
                courseId);
    }

    public async Task<EnrollmentDto?> GetActiveEnrollmentAsync(
        int userId,
        int courseId)
    {
        var enrollment =
            await _enrollmentRepository.GetActiveEnrollmentAsync(
                userId,
                courseId);

        if (enrollment == null)
            return null;

        return new EnrollmentDto
        {
            EnrollmentId = enrollment.EnrollmentId,
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = enrollment.Status,
            UserId = enrollment.UserId,
            CourseId = enrollment.CourseId
        };
    }

    public async Task DeleteAsync(
        int id,
        int userId)
    {
        var enrollment =
            await _enrollmentRepository.GetByIdAsync(id);

        if (enrollment == null)
            throw new KeyNotFoundException(
                "Enrollment not found.");

        if (enrollment.UserId != userId)
        {
            throw new UnauthorizedAccessException(
                "You are not allowed to delete this enrollment.");
        }

        _enrollmentRepository.Delete(enrollment);

        await _enrollmentRepository.SaveChangesAsync();
    }
}