using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;

namespace OnlineLearning.Web.Controllers;

[Authorize(Roles = "Student")]
public class FeedbackController : Controller
{
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(
        IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int userId = GetCurrentUserId();

        var feedbacks =
            await _feedbackService.GetAllAsync(userId);

        return View(feedbacks);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var feedback =
            await _feedbackService.GetByIdAsync(id);

        if (feedback == null)
            return NotFound();

        int userId = GetCurrentUserId();

        if (feedback.UserId != userId)
            return Forbid();

        return View(feedback);
    }

    [HttpGet]
    public IActionResult Create(int courseId)
    {
        var dto = new CreateFeedbackDto
        {
            CourseId = courseId
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateFeedbackDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        int userId = GetCurrentUserId();

        try
        {
            await _feedbackService.CreateAsync(
                dto,
                userId);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return View(dto);
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
            return View(dto);
        }

        TempData["Success"] =
            "Feedback submitted successfully and is waiting for review.";

        return RedirectToAction(
            nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var feedback =
            await _feedbackService.GetByIdAsync(id);

        if (feedback == null)
            return NotFound();

        int userId = GetCurrentUserId();

        if (feedback.UserId != userId)
            return Forbid();

        var dto = new CreateFeedbackDto
        {
            Rating = feedback.Rating,
            Comment = feedback.Comment,
            CourseId = feedback.CourseId
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CreateFeedbackDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        var feedback =
            await _feedbackService.GetByIdAsync(id);

        if (feedback == null)
            return NotFound();

        int userId = GetCurrentUserId();

        if (feedback.UserId != userId)
            return Forbid();

        try
        {
            await _feedbackService.UpdateAsync(
                id,
                dto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
            return View(dto);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return View(dto);
        }

        TempData["Success"] =
            "Feedback updated successfully and sent for review again.";

        return RedirectToAction(
            nameof(Details),
            new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var feedback =
            await _feedbackService.GetByIdAsync(id);

        if (feedback == null)
            return NotFound();

        int userId = GetCurrentUserId();

        if (feedback.UserId != userId)
            return Forbid();

        try
        {
            await _feedbackService.DeleteAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        TempData["Success"] =
            "Feedback deleted successfully.";

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