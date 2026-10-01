using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;

namespace OnlineLearning.Web.Controllers;

[Authorize(Roles = "Instructor")]
public class AnswerController : Controller
{
    private readonly IAnswerService _answerService;
    private readonly IQuestionService _questionService;
    private readonly IQuizService _quizService;
    private readonly IModuleService _moduleService;
    private readonly ICourseRepository _courseRepository;

    public AnswerController(
        IAnswerService answerService,
        IQuestionService questionService,
        IQuizService quizService,
        IModuleService moduleService,
        ICourseRepository courseRepository)
    {
        _answerService = answerService;
        _questionService = questionService;
        _quizService = quizService;
        _moduleService = moduleService;
        _courseRepository = courseRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int questionId)
    {
        var question =
            await _questionService.GetByIdAsync(questionId);

        if (question == null)
            return NotFound();

        var isOwner =
            await IsQuestionOwnedByInstructorAsync(
                question,
                GetCurrentUserId());

        if (!isOwner)
            return Forbid();

        ViewBag.QuestionId = questionId;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int questionId,
        CreateAnswerDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.QuestionId = questionId;
            return View(dto);
        }

        var question =
            await _questionService.GetByIdAsync(questionId);

        if (question == null)
            return NotFound();

        var isOwner =
            await IsQuestionOwnedByInstructorAsync(
                question,
                GetCurrentUserId());

        if (!isOwner)
            return Forbid();

        dto.QuestionId = questionId;

        try
        {
            await _answerService.CreateAsync(dto);

            return RedirectToAction(
                "Edit",
                "Question",
                new { id = questionId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var answer =
            await _answerService.GetByIdAsync(id);

        if (answer == null)
            return NotFound();

        var question =
            await _questionService.GetByIdAsync(
                answer.QuestionId);

        if (question == null)
            return NotFound();

        var isOwner =
            await IsQuestionOwnedByInstructorAsync(
                question,
                GetCurrentUserId());

        if (!isOwner)
            return Forbid();

        var dto = new CreateAnswerDto
        {
            AnswerText = answer.AnswerText,
            IsCorrect = answer.IsCorrect,
            QuestionId = answer.QuestionId
        };

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        CreateAnswerDto dto)
    {
        if (!ModelState.IsValid)
            return View(dto);

        var answer =
            await _answerService.GetByIdAsync(id);

        if (answer == null)
            return NotFound();

        var question =
            await _questionService.GetByIdAsync(
                answer.QuestionId);

        if (question == null)
            return NotFound();

        var isOwner =
            await IsQuestionOwnedByInstructorAsync(
                question,
                GetCurrentUserId());

        if (!isOwner)
            return Forbid();

        dto.QuestionId = answer.QuestionId;

        try
        {
            await _answerService.UpdateAsync(
                id,
                dto);

            return RedirectToAction(
                "Edit",
                "Question",
                new { id = answer.QuestionId });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var answer =
            await _answerService.GetByIdAsync(id);

        if (answer == null)
            return NotFound();

        var question =
            await _questionService.GetByIdAsync(
                answer.QuestionId);

        if (question == null)
            return NotFound();

        var isOwner =
            await IsQuestionOwnedByInstructorAsync(
                question,
                GetCurrentUserId());

        if (!isOwner)
            return Forbid();

        try
        {
            await _answerService.DeleteAsync(id);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(
            "Edit",
            "Question",
            new { id = answer.QuestionId });
    }

    private async Task<bool> IsQuestionOwnedByInstructorAsync(
        QuestionDto question,
        int instructorId)
    {
        var quiz =
            await _quizService.GetByIdAsync(
                question.QuizId);

        if (quiz == null)
            return false;

        var module =
            await _moduleService.GetByIdAsync(
                quiz.ModuleId);

        if (module == null)
            return false;

        var course =
            await _courseRepository.GetByIdAsync(
                module.CourseId);

        if (course == null)
            return false;

        return course.InstructorId == instructorId;
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