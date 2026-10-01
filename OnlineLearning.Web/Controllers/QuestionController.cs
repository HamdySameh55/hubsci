using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;

namespace OnlineLearning.Web.Controllers;

[Authorize(Roles = "Instructor")]
public class QuestionController : Controller
{
    private readonly IQuestionService _questionService;
    private readonly IQuizService _quizService;
    private readonly IModuleService _moduleService;
    private readonly ICourseRepository _courseRepository;

    public QuestionController(
        IQuestionService questionService,
        IQuizService quizService,
        IModuleService moduleService,
        ICourseRepository courseRepository)
    {
        _questionService = questionService;
        _quizService = quizService;
        _moduleService = moduleService;
        _courseRepository = courseRepository;
    }


    // =========================
    // Instructor - Create
    // =========================

    [HttpGet]
    public async Task<IActionResult> Create(int quizId)
    {
        var quiz =
            await _quizService.GetByIdAsync(quizId);

        if (quiz == null)
            return NotFound();

        var module =
            await _moduleService.GetByIdAsync(
                quiz.ModuleId);

        if (module == null)
            return NotFound();

        if (!await IsOwnerAsync(module.CourseId))
            return Forbid();

        ViewBag.QuizId = quizId;

        return View();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int quizId,
        CreateQuestionDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.QuizId = quizId;
            return View(dto);
        }

        var quiz =
            await _quizService.GetByIdAsync(quizId);

        if (quiz == null)
            return NotFound();

        var module =
            await _moduleService.GetByIdAsync(
                quiz.ModuleId);

        if (module == null)
            return NotFound();

        if (!await IsOwnerAsync(module.CourseId))
            return Forbid();

        dto.QuizId = quizId;

        try
        {
            await _questionService.CreateAsync(dto);

            return RedirectToAction(
                "Edit",
                "Quiz",
                new { id = quizId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }


    // =========================
    // Instructor - Edit
    // =========================

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var question =
            await _questionService.GetByIdAsync(id);

        if (question == null)
            return NotFound();

        var quiz =
            await _quizService.GetByIdAsync(
                question.QuizId);

        if (quiz == null)
            return NotFound();

        var module =
            await _moduleService.GetByIdAsync(
                quiz.ModuleId);

        if (module == null)
            return NotFound();

        if (!await IsOwnerAsync(module.CourseId))
            return Forbid();

        return View(question);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CreateQuestionDto dto)
    {
        if (!ModelState.IsValid)
        {
            var question =
                await _questionService.GetByIdAsync(id);

            if (question == null)
                return NotFound();

            return View(question);
        }

        var existingQuestion =
            await _questionService.GetByIdAsync(id);

        if (existingQuestion == null)
            return NotFound();

        var quiz =
            await _quizService.GetByIdAsync(
                existingQuestion.QuizId);

        if (quiz == null)
            return NotFound();

        var module =
            await _moduleService.GetByIdAsync(
                quiz.ModuleId);

        if (module == null)
            return NotFound();

        if (!await IsOwnerAsync(module.CourseId))
            return Forbid();

        dto.QuizId = existingQuestion.QuizId;

        try
        {
            await _questionService.UpdateAsync(
                id,
                dto);

            return RedirectToAction(
                nameof(Edit),
                new { id });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }


    // =========================
    // Instructor - Delete
    // =========================

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var question =
            await _questionService.GetByIdAsync(id);

        if (question == null)
            return NotFound();

        var quiz =
            await _quizService.GetByIdAsync(
                question.QuizId);

        if (quiz == null)
            return NotFound();

        var module =
            await _moduleService.GetByIdAsync(
                quiz.ModuleId);

        if (module == null)
            return NotFound();

        if (!await IsOwnerAsync(module.CourseId))
            return Forbid();

        try
        {
            await _questionService.DeleteAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(
            "Edit",
            "Quiz",
            new { id = question.QuizId });
    }


    // =========================
    // Ownership Check
    // =========================

    private async Task<bool> IsOwnerAsync(
        int courseId)
    {
        var course =
            await _courseRepository.GetByIdAsync(
                courseId);

        if (course == null)
            return false;

        int instructorId =
            GetCurrentUserId();

        return course.InstructorId == instructorId;
    }


    // =========================
    // Helper
    // =========================

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