using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;

namespace OnlineLearning.Web.Controllers;

[Authorize]
public class QuizController : Controller
{
    private readonly IQuizService _quizService;
    private readonly IEnrollmentService _enrollmentService;
    private readonly IModuleService _moduleService;
    private readonly ICourseRepository _courseRepository;

    public QuizController(
        IQuizService quizService,
        IEnrollmentService enrollmentService,
        IModuleService moduleService,
        ICourseRepository courseRepository)
    {
        _quizService = quizService;
        _enrollmentService = enrollmentService;
        _moduleService = moduleService;
        _courseRepository = courseRepository;
    }


    // =========================
    // Student / Admin - View Quiz
    // =========================

    [Authorize(Roles = "Student,Admin")]
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var quiz =
            await _quizService.GetByIdWithQuestionsAsync(id);

        if (quiz == null)
            return NotFound();

        // Admin can view any quiz
        if (User.IsInRole("Admin"))
        {
            return View(quiz);
        }

        // Student must have an active enrollment
        int userId = GetCurrentUserId();

        var enrollment =
            await _enrollmentService.GetActiveEnrollmentAsync(
                userId,
                quiz.CourseId);

        if (enrollment == null)
            return Forbid();

        ViewBag.EnrollmentId =
            enrollment.EnrollmentId;

        return View(quiz);
    }


    // =========================
    // Instructor - Create Quiz
    // =========================

    [Authorize(Roles = "Instructor")]
    [HttpGet]
    public async Task<IActionResult> Create(int moduleId)
    {
        var module =
            await _moduleService.GetByIdAsync(moduleId);

        if (module == null)
            return NotFound();

        int instructorId = GetCurrentUserId();

        var courseOwner =
            await IsModuleOwnedByInstructorAsync(
                module,
                instructorId);

        if (!courseOwner)
            return Forbid();

        ViewBag.ModuleId = moduleId;

        return View();
    }


    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int moduleId,
        CreateQuizDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ModuleId = moduleId;

            return View(dto);
        }

        var module =
            await _moduleService.GetByIdAsync(moduleId);

        if (module == null)
            return NotFound();

        int instructorId = GetCurrentUserId();

        var courseOwner =
            await IsModuleOwnedByInstructorAsync(
                module,
                instructorId);

        if (!courseOwner)
            return Forbid();

        dto.ModuleId = moduleId;

        try
        {
            var quiz =
                await _quizService.CreateAsync(dto);

            return RedirectToAction(
                nameof(Edit),
                new { id = quiz.QuizId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }


    // =========================
    // Instructor - Edit Quiz
    // =========================

    [Authorize(Roles = "Instructor")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var quiz =
            await _quizService.GetByIdWithQuestionsAsync(id);

        if (quiz == null)
            return NotFound();

        var module =
            await _moduleService.GetByIdAsync(
                quiz.ModuleId);

        if (module == null)
            return NotFound();

        int instructorId = GetCurrentUserId();

        var courseOwner =
            await IsModuleOwnedByInstructorAsync(
                module,
                instructorId);

        if (!courseOwner)
            return Forbid();

        return View(quiz);
    }


    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CreateQuizDto dto)
    {
        if (!ModelState.IsValid)
        {
            var quizWithQuestions =
                await _quizService.GetByIdWithQuestionsAsync(id);

            if (quizWithQuestions == null)
                return NotFound();

            return View(quizWithQuestions);
        }

        var quiz =
            await _quizService.GetByIdAsync(id);

        if (quiz == null)
            return NotFound();

        var module =
            await _moduleService.GetByIdAsync(
                quiz.ModuleId);

        if (module == null)
            return NotFound();

        int instructorId = GetCurrentUserId();

        var courseOwner =
            await IsModuleOwnedByInstructorAsync(
                module,
                instructorId);

        if (!courseOwner)
            return Forbid();

        dto.ModuleId = quiz.ModuleId;

        try
        {
            await _quizService.UpdateAsync(
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
    // Instructor - Delete Quiz
    // =========================

    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var quiz =
            await _quizService.GetByIdAsync(id);

        if (quiz == null)
            return NotFound();

        var module =
            await _moduleService.GetByIdAsync(
                quiz.ModuleId);

        if (module == null)
            return NotFound();

        int instructorId = GetCurrentUserId();

        var courseOwner =
            await IsModuleOwnedByInstructorAsync(
                module,
                instructorId);

        if (!courseOwner)
            return Forbid();

        try
        {
            await _quizService.DeleteAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(
                "Details",
                "Module",
                new { id = module.ModuleId });
        }

        return RedirectToAction(
            "Details",
            "Module",
            new { id = module.ModuleId });
    }


    // =========================
    // Ownership Check
    // =========================

    private async Task<bool> IsModuleOwnedByInstructorAsync(
        ModuleDto module,
        int instructorId)
    {
        var course =
            await _courseRepository.GetByIdAsync(
                module.CourseId);

        if (course == null)
            return false;

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