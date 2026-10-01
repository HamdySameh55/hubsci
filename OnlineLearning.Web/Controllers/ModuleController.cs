using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;

namespace OnlineLearning.Web.Controllers;

[Authorize]
public class ModuleController : Controller
{
    private readonly IModuleService _moduleService;
    private readonly ICourseRepository _courseRepository;
    private readonly ILessonService _lessonService;
    private readonly IQuizService _quizService;
    private readonly IEnrollmentService _enrollmentService;

    public ModuleController(
        IModuleService moduleService,
        ICourseRepository courseRepository,
        ILessonService lessonService,
        IQuizService quizService,
        IEnrollmentService enrollmentService)
    {
        _moduleService = moduleService;
        _courseRepository = courseRepository;
        _lessonService = lessonService;
        _quizService = quizService;
        _enrollmentService = enrollmentService;
    }

    // =========================
    // View modules of a course
    // =========================

    [HttpGet]
    public async Task<IActionResult> Index(int courseId)
    {
        var course =
            await _courseRepository.GetByIdAsync(courseId);

        if (course == null)
            return NotFound();

        bool isOwner = false;

        // Instructor
        if (User.IsInRole("Instructor"))
        {
            int instructorId = GetCurrentUserId();

            if (course.InstructorId != instructorId)
                return Forbid();

            isOwner = true;
        }

        // Student
        else if (User.IsInRole("Student"))
        {
            int userId = GetCurrentUserId();

            var enrollment =
                await _enrollmentService.GetActiveEnrollmentAsync(
                    userId,
                    courseId);

            if (enrollment == null)
                return Forbid();
        }

        // Admin
        else if (User.IsInRole("Admin"))
        {
            // Admin can view modules of any course.
        }

        // Any other role
        else
        {
            return Forbid();
        }

        var modules =
            await _moduleService.GetByCourseIdAsync(courseId);

        ViewBag.CourseId = courseId;
        ViewBag.IsOwner = isOwner;

        return View(modules);
    }


    // =========================
    // View module details
    // =========================

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var module =
            await _moduleService.GetByIdAsync(id);

        if (module == null)
            return NotFound();

        bool isOwner = false;

        // Instructor
        if (User.IsInRole("Instructor"))
        {
            int instructorId = GetCurrentUserId();

            var course =
                await _courseRepository.GetByIdAsync(
                    module.CourseId);

            if (course == null)
                return NotFound();

            if (course.InstructorId != instructorId)
                return Forbid();

            isOwner = true;
        }

        // Student
        else if (User.IsInRole("Student"))
        {
            int userId = GetCurrentUserId();

            var enrollment =
                await _enrollmentService.GetActiveEnrollmentAsync(
                    userId,
                    module.CourseId);

            if (enrollment == null)
                return Forbid();
        }

        // Admin
        else if (User.IsInRole("Admin"))
        {
            // Admin can view module details,
            // lessons, and quiz of any course.
        }

        // Any other role
        else
        {
            return Forbid();
        }


        // =========================
        // Lessons
        // =========================

        var allLessons =
            await _lessonService.GetAllAsync();

        var moduleLessons =
            allLessons
                .Where(lesson =>
                    lesson.ModuleId == module.ModuleId)
                .ToList();

        ViewBag.Lessons = moduleLessons;


        // =========================
        // Quiz
        // =========================

        var allQuizzes =
            await _quizService.GetAllAsync();

        var moduleQuiz =
            allQuizzes
                .FirstOrDefault(quiz =>
                    quiz.ModuleId == module.ModuleId);

        ViewBag.Quiz = moduleQuiz;


        // =========================
        // Instructor ownership
        // =========================

        ViewBag.IsOwner = isOwner;

        return View(module);
    }


    // =========================
    // Instructor - Create Module
    // =========================

    [Authorize(Roles = "Instructor")]
    [HttpGet]
    public IActionResult Create(int courseId)
    {
        ViewBag.CourseId = courseId;

        return View();
    }


    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int courseId,
        CreateModuleDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.CourseId = courseId;

            return View(dto);
        }

        int instructorId =
            GetCurrentUserId();

        try
        {
            await _moduleService.CreateAsync(
                dto,
                courseId,
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
            ModelState.AddModelError(
                "OrderNumber",
                ex.Message);

            ViewBag.CourseId = courseId;

            return View(dto);
        }

        return RedirectToAction(
            nameof(Index),
            new { courseId });
    }


    // =========================
    // Instructor - Edit Module
    // =========================

    [Authorize(Roles = "Instructor")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var module =
            await _moduleService.GetByIdAsync(id);

        if (module == null)
            return NotFound();

        var dto = new CreateModuleDto
        {
            Title = module.Title,
            Description = module.Description,
            OrderNumber = module.OrderNumber
        };

        ViewBag.CourseId = module.CourseId;

        return View(dto);
    }


    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CreateModuleDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        int instructorId =
            GetCurrentUserId();

        try
        {
            await _moduleService.UpdateAsync(
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
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(
                "OrderNumber",
                ex.Message);

            return View(dto);
        }

        var module =
            await _moduleService.GetByIdAsync(id);

        if (module == null)
            return NotFound();

        return RedirectToAction(
            nameof(Index),
            new { courseId = module.CourseId });
    }


    // =========================
    // Instructor - Delete Module
    // =========================

    [Authorize(Roles = "Instructor")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        int instructorId =
            GetCurrentUserId();

        ModuleDto? module =
            await _moduleService.GetByIdAsync(id);

        if (module == null)
            return NotFound();

        try
        {
            await _moduleService.DeleteAsync(
                id,
                instructorId);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(
                nameof(Index),
                new { courseId = module.CourseId });
        }

        return RedirectToAction(
            nameof(Index),
            new { courseId = module.CourseId });
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