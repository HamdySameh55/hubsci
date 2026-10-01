using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface ICourseProgressService
{
    Task<CourseProgressDto?> GetProgressAsync(int enrollmentId);
}