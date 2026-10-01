using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;

namespace OnlineLearning.Business.Services.Implementations;

public class AdminDashboardService : IAdminDashboardService
{
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IPaymentRepository _paymentRepository;

    public AdminDashboardService(
        IUserRepository userRepository,
        ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository,
        IPaymentRepository paymentRepository)
    {
        _userRepository = userRepository;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _paymentRepository = paymentRepository;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync()
    {
        var users =
            (await _userRepository.GetAllAsync()).ToList();

        var courses =
            (await _courseRepository.GetAllAsync()).ToList();

        var enrollments =
            (await _enrollmentRepository.GetAllAsync()).ToList();

        var payments =
            (await _paymentRepository.GetAllAsync()).ToList();

        return new AdminDashboardDto
        {
            TotalUsers = users.Count,

            TotalStudents = users.Count(user =>
                user.Role == "Student"),

            TotalInstructors = users.Count(user =>
                user.Role == "Instructor"),

            TotalCourses = courses.Count,

            PublishedCourses = courses.Count(course =>
                course.Status == "Published"),

            TotalEnrollments = enrollments.Count,

            TotalRevenue = payments
                .Where(payment =>
                    payment.PaymentStatus == "Successful")
                .Sum(payment =>
                    payment.Amount)
        };
    }
}