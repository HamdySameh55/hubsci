using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.Services.Interfaces;

namespace OnlineLearning.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IAdminDashboardService _adminDashboardService;
    private readonly ICourseService _courseService;
    private readonly IFeedbackService _feedbackService;
    private readonly ILessonService _lessonService;

    public AdminController(
        IAdminDashboardService adminDashboardService,
        ICourseService courseService,
        IFeedbackService feedbackService,
        ILessonService lessonService)
    {
        _adminDashboardService = adminDashboardService;
        _courseService = courseService;
        _feedbackService = feedbackService;
        _lessonService = lessonService;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var dashboard =
            await _adminDashboardService.GetDashboardAsync();

        return View(dashboard);
    }

    [HttpGet]
    public async Task<IActionResult> Courses()
    {
        var courses =
            await _courseService.GetAllAsync();

        return View(courses);
    }

    [HttpGet]
    public async Task<IActionResult> Feedback()
    {
        var feedbacks =
            await _feedbackService
                .GetAllForAdminAsync();

        return View(feedbacks);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        try
        {
            await _courseService.DeleteByAdminAsync(id);

            return RedirectToAction(nameof(Courses));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLesson(int id)
    {
        var lesson =
            await _lessonService.GetByIdAsync(id);

        if (lesson == null)
            return NotFound();

        try
        {
            await _lessonService.DeleteByAdminAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(
            "Details",
            "Module",
            new { id = lesson.ModuleId });
    }
}