using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;

namespace OnlineLearning.Web.ViewModels;

public class SubmitPaymentProofViewModel
{
    public int PaymentId { get; set; }

    public int EnrollmentId { get; set; }

    [Required]
    [Display(Name = "Transaction Reference")]
    public string TransactionReference { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Payment Screenshot")]
    public IFormFile ProofFile { get; set; } = null!;
}
