using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;

namespace OnlineLearning.Web.Controllers;

[Authorize(Roles = "Student")]
public class QuizAttemptController : Controller
{
    private readonly IQuizAttemptService _quizAttemptService;
    private readonly IEnrollmentService _enrollmentService;

    public QuizAttemptController(
        IQuizAttemptService quizAttemptService,
        IEnrollmentService enrollmentService)
    {
        _quizAttemptService = quizAttemptService;
        _enrollmentService = enrollmentService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        CreateQuizAttemptDto dto)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] =
                "Invalid quiz submission.";

            return RedirectToAction(
                "Details",
                "Quiz",
                new { id = dto.QuizId });
        }

        try
        {
            int userId = GetCurrentUserId();

            // Make sure the enrollment belongs
            // to the currently logged-in student.
            var enrollment =
                await _enrollmentService.GetByIdAsync(
                    dto.EnrollmentId);

            if (enrollment == null)
                return NotFound();

            if (enrollment.UserId != userId)
                return Forbid();

            if (enrollment.Status != "Active")
            {
                TempData["Error"] =
                    "You must have an active enrollment to take this quiz.";

                return RedirectToAction(
                    "Details",
                    "Quiz",
                    new { id = dto.QuizId });
            }

            var attempt =
                await _quizAttemptService.CreateAsync(dto);

            return View("Result", attempt);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(
                "Details",
                "Quiz",
                new { id = dto.QuizId });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(
                "Details",
                "Quiz",
                new { id = dto.QuizId });
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(
                "Details",
                "Quiz",
                new { id = dto.QuizId });
        }
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