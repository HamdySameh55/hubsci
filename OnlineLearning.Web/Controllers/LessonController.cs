using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;

namespace OnlineLearning.Web.Controllers;

public class LessonController : Controller
{
    private readonly ILessonService _lessonService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly ILessonProgressService _lessonProgressService;

    public LessonController(
        ILessonService lessonService,
        IEnrollmentService enrollmentService,
        ILessonProgressService lessonProgressService)
    {
        _lessonService = lessonService;
        _enrollmentService = enrollmentService;
        _lessonProgressService = lessonProgressService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var lessons =
            await _lessonService.GetAllAsync();

        return View(lessons);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var lesson =
            await _lessonService.GetByIdAsync(id);

        if (lesson == null)
            return NotFound();

        int userId = GetCurrentUserId();

        // Instructors and Admins can access lesson details
        if (User.IsInRole("Instructor") ||
            User.IsInRole("Admin"))
        {
            return View(lesson);
        }

        // Students must have an active enrollment
        if (User.IsInRole("Student"))
        {
            var enrollment =
                await _enrollmentService
                    .GetActiveEnrollmentAsync(
                        userId,
                        lesson.CourseId);

            if (enrollment == null)
                return Forbid();

            // Send EnrollmentId to the View
            ViewBag.EnrollmentId =
                enrollment.EnrollmentId;

            // Check if this lesson is already completed
            var isCompleted =
                await _lessonProgressService
                    .IsLessonCompletedAsync(
                        enrollment.EnrollmentId,
                        lesson.LessonId);

            // Send completion status to the View
            ViewBag.IsCompleted =
                isCompleted;

            return View(lesson);
        }

        return Forbid();
    }

    [Authorize(Roles = "Instructor")]
    [HttpGet]
    public IActionResult Create(int moduleId)
    {
        ViewBag.ModuleId = moduleId;

        return View();
    }

    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int moduleId,
        CreateLessonDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ModuleId = moduleId;

            return View(dto);
        }

        int instructorId =
            GetCurrentUserId();

        dto.ModuleId = moduleId;

        try
        {
            await _lessonService.CreateAsync(
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
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var lesson =
            await _lessonService.GetByIdAsync(id);

        if (lesson == null)
            return NotFound();

        var dto = new CreateLessonDto
        {
            Title = lesson.Title,
            Description = lesson.Description,
            ContentType = lesson.ContentType,
            ContentUrl = lesson.ContentUrl,
            ModuleId = lesson.ModuleId
        };

        ViewBag.ModuleId =
            lesson.ModuleId;

        return View(dto);
    }

    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CreateLessonDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        int instructorId =
            GetCurrentUserId();

        try
        {
            await _lessonService.UpdateAsync(
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
            nameof(Details),
            new { id });
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
            await _lessonService.DeleteAsync(
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

            return RedirectToAction(
                nameof(Details),
                new { id });
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