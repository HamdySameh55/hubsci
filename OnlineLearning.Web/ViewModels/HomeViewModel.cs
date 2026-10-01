using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Web.ViewModels;

public class HomeViewModel
{
    public IEnumerable<CourseDto> Courses { get; set; }
        = Enumerable.Empty<CourseDto>();

    public StudentHomeViewModel? Student { get; set; }
}

public class StudentHomeViewModel
{
    public CourseDto? Course { get; set; }

    public CourseProgressDto? Progress { get; set; }
}