
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;

namespace OnlineLearning.Web.Controllers;

public class CourseController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly ICourseProgressService _courseProgressService;
    private readonly ICertificateService _certificateService;

    public CourseController(
        ICourseService courseService,
        IEnrollmentService enrollmentService,
        ICourseProgressService courseProgressService,
        ICertificateService certificateService)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
        _courseProgressService = courseProgressService;
        _certificateService = certificateService;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Instructor"))
        {
            int instructorId = GetCurrentUserId();

            var instructorCourses =
                await _courseService.GetByInstructorAsync(
                    instructorId);

            return View(instructorCourses);
        }

        var courses =
            await _courseService.GetPublishedAsync();

        return View(courses);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var course =
            await _courseService.GetPublishedByIdAsync(id);

        if (course == null)
            return NotFound();

        bool canGiveFeedback = false;
        bool canGetCertificate = false;
        bool hasCertificate = false;

        if (User.IsInRole("Student"))
        {
            int userId = GetCurrentUserId();

            var enrollment =
                await _enrollmentService
                    .GetActiveEnrollmentAsync(
                        userId,
                        course.CourseId);

            if (enrollment != null)
            {
                var progress =
                    await _courseProgressService
                        .GetProgressAsync(
                            enrollment.EnrollmentId);

                // Debug information
                Console.WriteLine(
                    $"Enrollment: {enrollment.EnrollmentId}, " +
                    $"Completed: {progress?.IsCompleted}, " +
                    $"Lessons: {progress?.CompletedLessons}/{progress?.TotalLessons}, " +
                    $"Quizzes: {progress?.PassedQuizzes}/{progress?.TotalQuizzes}");

                if (progress != null &&
                    progress.IsCompleted)
                {
                    ViewBag.EnrollmentId =
                        enrollment.EnrollmentId;

                    canGiveFeedback = true;

                    var certificates =
                        await _certificateService
                            .GetAllAsync();

                    hasCertificate =
                        certificates.Any(c =>
                            c.EnrollmentId ==
                            enrollment.EnrollmentId);

                    canGetCertificate =
                        !hasCertificate;
                }
            }
        }

        ViewBag.CanGiveFeedback =
            canGiveFeedback;

        ViewBag.CanGetCertificate =
            canGetCertificate;

        ViewBag.HasCertificate =
            hasCertificate;

        return View(course);
    }

    [Authorize(Roles = "Instructor")]
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateCourseDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        int instructorId =
            GetCurrentUserId();

        await _courseService.CreateAsync(
            dto,
            instructorId);

        return RedirectToAction(
            nameof(Index));
    }

    [Authorize(Roles = "Instructor")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var course =
            await _courseService.GetByIdAsync(id);

        if (course == null)
            return NotFound();

        int instructorId =
            GetCurrentUserId();

        if (course.InstructorId != instructorId)
            return Forbid();

        var dto = new CreateCourseDto
        {
            Title =
                course.Title,

            Description =
                course.Description,

            Price =
                course.Price
        };

        return View(dto);
    }

    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CreateCourseDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        int instructorId =
            GetCurrentUserId();

        try
        {
            await _courseService.UpdateAsync(
                id,
                dto,
                instructorId);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        return RedirectToAction(
            nameof(Index));
    }

    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        int instructorId =
            GetCurrentUserId();

        try
        {
            await _courseService.DeleteAsync(
                id,
                instructorId);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(
            nameof(Index));
    }

    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id)
    {
        int instructorId =
            GetCurrentUserId();

        try
        {
            await _courseService.PublishAsync(
                id,
                instructorId);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }

        return RedirectToAction(
            nameof(Index));
    }

    private int GetCurrentUserId()
    {
        var userId =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException(
                "User ID was not found.");
        }

        return int.Parse(userId);
    }
}
