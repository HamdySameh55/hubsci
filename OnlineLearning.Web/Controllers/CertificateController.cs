using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.Services.Interfaces;

namespace OnlineLearning.Web.Controllers;

[Authorize(Roles = "Student")]
public class CertificateController : Controller
{
    private readonly ICertificateService _certificateService;
    private readonly IEnrollmentService _enrollmentService;

    public CertificateController(
        ICertificateService certificateService,
        IEnrollmentService enrollmentService)
    {
        _certificateService = certificateService;
        _enrollmentService = enrollmentService;
    }

    // GET: /Certificate
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        int userId = GetCurrentUserId();

        var certificates =
            await _certificateService.GetAllAsync();

        var userCertificates =
            new List<OnlineLearning.Business.DTOs.CertificateDto>();

        foreach (var certificate in certificates)
        {
            var enrollment =
                await _enrollmentService
                    .GetByIdAsync(certificate.EnrollmentId);

            if (enrollment != null &&
                enrollment.UserId == userId)
            {
                userCertificates.Add(certificate);
            }
        }

        return View(userCertificates);
    }

    // GET: /Certificate/Details/5
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var certificate =
            await _certificateService.GetByIdAsync(id);

        if (certificate == null)
            return NotFound();

        int userId = GetCurrentUserId();

        var enrollment =
            await _enrollmentService
                .GetByIdAsync(certificate.EnrollmentId);

        if (enrollment == null)
            return NotFound();

        if (enrollment.UserId != userId)
            return Forbid();

        return View(certificate);
    }

    // POST: /Certificate/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        int enrollmentId)
    {
        int userId = GetCurrentUserId();

        try
        {
            var certificate =
                await _certificateService.CreateAsync(
                    enrollmentId,
                    userId);

            TempData["Success"] =
                "Certificate issued successfully.";

            return RedirectToAction(
                nameof(Details),
                new
                {
                    id = certificate.CertificateId
                });
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData["Error"] = ex.Message;
            return Forbid();
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
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