using System.Diagnostics;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Web.Models;
using OnlineLearning.Web.ViewModels;

namespace OnlineLearning.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly ICourseProgressService _courseProgressService;

    public HomeController(
        ILogger<HomeController> logger,
        ICourseService courseService,
        IEnrollmentService enrollmentService,
        ICourseProgressService courseProgressService)
    {
        _logger = logger;
        _courseService = courseService;
        _enrollmentService = enrollmentService;
        _courseProgressService = courseProgressService;
    }


    // ==========================================
    // Public Home Page
    // ==========================================

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var courses =
            await _courseService.GetPublishedAsync();

        var model = new HomeViewModel
        {
            Courses = courses
        };

        // ==========================================
        // Student Home Data
        // ==========================================

        if (User.IsInRole("Student"))
        {
            var userIdClaim =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userIdClaim, out int userId))
            {
                var enrollments =
                    await _enrollmentService
                        .GetByUserIdAsync(userId);

                var activeEnrollment =
                    enrollments
                        .Where(e => e.Status == "Active")
                        .OrderByDescending(e => e.EnrollmentDate)
                        .FirstOrDefault();

                if (activeEnrollment != null)
                {
                    var course =
                        await _courseService
                            .GetByIdAsync(activeEnrollment.CourseId);

                    var progress =
                        await _courseProgressService
                            .GetProgressAsync(
                                activeEnrollment.EnrollmentId);

                    model.Student = new StudentHomeViewModel
                    {
                        Course = course,
                        Progress = progress
                    };
                }
            }
        }

        return View(model);
    }


    // ==========================================
    // Protected Page
    // ==========================================

    [Authorize]
    public IActionResult Profile()
    {
        return View();
    }


    // ==========================================
    // Admin Only Page
    // ==========================================

    [Authorize(Roles = "Admin")]
    public IActionResult Admin()
    {
        return View();
    }


    // ==========================================
    // Privacy
    // ==========================================

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }


    // ==========================================
    // Error
    // ==========================================

    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
    }
}