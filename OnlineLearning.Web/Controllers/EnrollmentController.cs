using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;

namespace OnlineLearning.Web.Controllers;

public class EnrollmentController : Controller
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(
        IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [Authorize(Roles = "Student")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int userId = GetCurrentUserId();

        var enrollments =
            await _enrollmentService.GetByUserIdAsync(userId);

        return View(enrollments);
    }

    [Authorize(Roles = "Student")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateEnrollmentDto dto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            TempData["Error"] =
                string.Join(" | ", errors);

            return RedirectToAction(
                "Details",
                "Course",
                new { id = dto.CourseId });
        }

        int userId = GetCurrentUserId();

        try
        {
            var enrollment =
                await _enrollmentService.CreateAsync(
                    dto,
                    userId);

            return RedirectToAction(
                "Details",
                "Enrollment",
                new { id = enrollment.EnrollmentId });
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
                "Details",
                "Course",
                new { id = dto.CourseId });
        }
    }

    [Authorize(Roles = "Student")]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var enrollment =
            await _enrollmentService.GetByIdAsync(id);

        if (enrollment == null)
            return NotFound();

        int userId = GetCurrentUserId();

        if (enrollment.UserId != userId)
            return Forbid();

        return View(enrollment);
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