using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Web.Services;
using OnlineLearning.Web.ViewModels;

namespace OnlineLearning.Web.Controllers;

public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly PaymentProofUploader _uploader;

    public PaymentController(
        IPaymentService paymentService,
        PaymentProofUploader uploader)
    {
        _paymentService = paymentService;
        _uploader = uploader;
    }


    // ─────────────────────────────────────────────
    //  Student — InstaPay payment page
    // ─────────────────────────────────────────────

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Details(int enrollmentId)
    {
        var userId = GetCurrentUserId();

        try
        {
            var payment =
                await _paymentService
                    .CreatePendingAsync(enrollmentId, userId);

            return View(payment);
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;

            return RedirectToAction(
                "Details",
                "Enrollment",
                new { id = enrollmentId });
        }
    }


    // ─────────────────────────────────────────────
    //  Student — submit proof
    // ─────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> SubmitProof(
        SubmitPaymentProofViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in all fields and attach a screenshot.";
            return RedirectToAction(
                nameof(Details),
                new { enrollmentId = model.EnrollmentId });
        }

        if (model.ProofFile == null ||
            model.ProofFile.Length == 0)
        {
            TempData["Error"] = "Please attach a payment screenshot.";
            return RedirectToAction(
                nameof(Details),
                new { enrollmentId = model.EnrollmentId });
        }

        try
        {
            var filePath =
                await _uploader.UploadAsync(model.ProofFile);

            var userId = GetCurrentUserId();

            await _paymentService.SubmitProofAsync(
                model.PaymentId,
                model.TransactionReference,
                filePath,
                userId);

            TempData["Success"] =
                "Payment proof submitted. Waiting for instructor verification.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(
            nameof(Details),
            new { enrollmentId = model.EnrollmentId });
    }


    // ─────────────────────────────────────────────
    //  Instructor — pending payments
    // ─────────────────────────────────────────────

    [HttpGet]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> Pending()
    {
        var instructorId = GetCurrentUserId();
        var payments =
            await _paymentService
                .GetPendingForInstructorAsync(instructorId);

        return View(payments);
    }


    // ─────────────────────────────────────────────
    //  Instructor — approve
    // ─────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            var instructorId = GetCurrentUserId();

            await _paymentService.ApproveAsync(id, instructorId);

            TempData["Success"] = "Payment approved. Enrollment is now active.";
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Pending));
    }


    // ─────────────────────────────────────────────
    //  Instructor — reject
    // ─────────────────────────────────────────────

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> Reject(int id)
    {
        try
        {
            var instructorId = GetCurrentUserId();

            await _paymentService.RejectAsync(id, instructorId);

            TempData["Success"] = "Payment rejected.";
        }
        catch (KeyNotFoundException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (UnauthorizedAccessException ex)
        {
            TempData["Error"] = ex.Message;
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Pending));
    }


    // ─────────────────────────────────────────────
    //  Helpers
    // ─────────────────────────────────────────────

    private int GetCurrentUserId()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException(
                "User ID was not found.");

        return int.Parse(userId);
    }
}
