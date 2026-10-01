using System.Net.Mime;

namespace OnlineLearning.Web.Services;

public class PaymentProofUploader
{
    private readonly IWebHostEnvironment _env;
    private const long MaxFileSize = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions =
        [".jpg", ".jpeg", ".png", ".gif", ".webp"];

    public PaymentProofUploader(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (string.IsNullOrEmpty(ext) ||
            !AllowedExtensions.Contains(ext))
        {
            throw new InvalidOperationException(
                $"Invalid file type. Allowed types: {string.Join(", ", AllowedExtensions)}");
        }

        if (file.Length > MaxFileSize)
            throw new InvalidOperationException(
                "File size must be 5 MB or less.");

        var uploadsDir =
            Path.Combine(_env.WebRootPath, "uploads", "proofs");

        Directory.CreateDirectory(uploadsDir);

        var safeName = $"{Guid.NewGuid()}{ext}";

        var fullPath = Path.Combine(uploadsDir, safeName);

        using (var stream = File.Create(fullPath))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/proofs/{safeName}";
    }
}
