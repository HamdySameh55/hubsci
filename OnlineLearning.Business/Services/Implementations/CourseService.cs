using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class CourseService : ICourseService
{
    private readonly ICourseRepository _courseRepository;

    public CourseService(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<CourseDto>> GetByInstructorAsync(
        int instructorId)
    {
        var courses = await _courseRepository.GetAllAsync();

        return courses
            .Where(course => course.InstructorId == instructorId)
            .Select(course => new CourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                Status = course.Status,
                Price = course.Price,
                InstructorId = course.InstructorId
            });
    }

    public async Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        var courses = await _courseRepository.GetAllAsync();

        return courses.Select(course => new CourseDto
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            Status = course.Status,
            Price = course.Price,
            InstructorId = course.InstructorId
        });
    }

    public async Task<CourseDto?> GetPublishedByIdAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
            return null;

        if (course.Status != "Published")
            return null;

        return new CourseDto
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            Status = course.Status,
            Price = course.Price,
            InstructorId = course.InstructorId
        };
    }

    public async Task<IEnumerable<CourseDto>> GetPublishedAsync()
    {
        var courses = await _courseRepository.GetAllAsync();

        return courses
            .Where(course => course.Status == "Published")
            .Select(course => new CourseDto
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                Status = course.Status,
                Price = course.Price,
                InstructorId = course.InstructorId
            });
    }

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
            return null;

        return new CourseDto
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            Status = course.Status,
            Price = course.Price,
            InstructorId = course.InstructorId
        };
    }

    public async Task<CourseDto> CreateAsync(
        CreateCourseDto dto,
        int instructorId)
    {
        var course = new Course
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            InstructorId = instructorId,
            Status = "Draft"
        };

        await _courseRepository.AddAsync(course);
        await _courseRepository.SaveChangesAsync();

        return new CourseDto
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            Status = course.Status,
            Price = course.Price,
            InstructorId = course.InstructorId
        };
    }

    public async Task UpdateAsync(
        int id,
        CreateCourseDto dto,
        int instructorId)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
            throw new KeyNotFoundException("Course not found.");

        if (course.InstructorId != instructorId)
            throw new UnauthorizedAccessException(
                "You are not allowed to modify this course.");

        course.Title = dto.Title;
        course.Description = dto.Description;
        course.Price = dto.Price;

        _courseRepository.Update(course);

        await _courseRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        int id,
        int instructorId)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
            throw new KeyNotFoundException("Course not found.");

        if (course.InstructorId != instructorId)
            throw new UnauthorizedAccessException(
                "You are not allowed to delete this course.");

        if (await _courseRepository.HasEnrollmentsAsync(id))
            throw new InvalidOperationException(
                "This course has enrolled students and cannot be deleted.");

        _courseRepository.Delete(course);

        await _courseRepository.SaveChangesAsync();
    }

    public async Task DeleteByAdminAsync(int id)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
            throw new KeyNotFoundException("Course not found.");

        if (await _courseRepository.HasEnrollmentsAsync(id))
            throw new InvalidOperationException(
                "This course has enrolled students and cannot be deleted.");

        _courseRepository.Delete(course);

        await _courseRepository.SaveChangesAsync();
    }

    public async Task PublishAsync(
        int id,
        int instructorId)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course == null)
            throw new KeyNotFoundException("Course not found.");

        if (course.InstructorId != instructorId)
            throw new UnauthorizedAccessException(
                "You are not allowed to publish this course.");

        course.Status = "Published";

        _courseRepository.Update(course);

        await _courseRepository.SaveChangesAsync();
    }
}