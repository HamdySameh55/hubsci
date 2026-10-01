using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;

namespace OnlineLearning.Web.Controllers;

[Authorize(Roles = "Student")]
public class LessonProgressController : Controller
{
    private readonly ILessonProgressService _lessonProgressService;

    public LessonProgressController(
        ILessonProgressService lessonProgressService)
    {
        _lessonProgressService = lessonProgressService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int userId = GetCurrentUserId();

        var progress =
            await _lessonProgressService.GetByUserIdAsync(userId);

        return View(progress);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        int userId = GetCurrentUserId();

        var progress =
            await _lessonProgressService.GetByIdForUserAsync(
                id,
                userId);

        if (progress == null)
            return NotFound();

        return View(progress);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateLessonProgressDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] =
                "Invalid lesson progress data.";

            return RedirectToAction(
                "Details",
                "Lesson",
                new { id = dto.LessonId });
        }

        int userId = GetCurrentUserId();

        try
        {
            await _lessonProgressService.CreateAsync(
                dto,
                userId);
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(
                "Details",
                "Lesson",
                new { id = dto.LessonId });
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData["Error"] = ex.Message;

            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(
                "Details",
                "Lesson",
                new { id = dto.LessonId });
        }

        TempData["Success"] =
            "Lesson completed successfully.";

        return RedirectToAction(
            "Details",
            "Lesson",
            new { id = dto.LessonId });
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