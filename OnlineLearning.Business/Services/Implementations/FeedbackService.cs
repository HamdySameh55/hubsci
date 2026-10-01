using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;
using OnlineLearning.Data.Repositories.Interfaces;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class FeedbackService : IFeedbackService
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICourseProgressService _courseProgressService;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public FeedbackService(
        IFeedbackRepository feedbackRepository,
        IEnrollmentRepository enrollmentRepository,
        ICourseProgressService courseProgressService,
        IUserRepository userRepository,
        ICourseRepository courseRepository)
    {
        _feedbackRepository = feedbackRepository;
        _enrollmentRepository = enrollmentRepository;
        _courseProgressService = courseProgressService;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<IEnumerable<FeedbackDto>> GetAllAsync(
        int userId)
    {
        var feedbacks =
            await _feedbackRepository
                .GetByUserIdAsync(userId);

        return feedbacks.Select(MapToDto);
    }

    public async Task<IEnumerable<FeedbackDto>> GetAllForAdminAsync()
    {
        var feedbacks =
            await _feedbackRepository
                .GetAllAsync();

        var dtos = new List<FeedbackDto>();

        foreach (var feedback in feedbacks)
        {
            var dto = MapToDto(feedback);

            var user =
                await _userRepository
                    .GetByIdAsync(feedback.UserId);

            var course =
                await _courseRepository
                    .GetByIdAsync(feedback.CourseId);

            dto.StudentName =
                user?.Name
                ?? $"User #{feedback.UserId}";

            dto.CourseTitle =
                course?.Title
                ?? $"Course #{feedback.CourseId}";

            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<FeedbackDto?> GetByIdAsync(
        int id)
    {
        var feedback =
            await _feedbackRepository
                .GetByIdAsync(id);

        if (feedback == null)
            return null;

        return MapToDto(feedback);
    }

    public async Task<FeedbackDto> CreateAsync(
        CreateFeedbackDto dto,
        int userId)
    {
        // 1. Check enrollment
        var enrollment =
            await _enrollmentRepository
                .GetByUserAndCourseAsync(
                    userId,
                    dto.CourseId);

        if (enrollment == null)
        {
            throw new InvalidOperationException(
                "Student is not enrolled in this course.");
        }

        // 2. Enrollment must be active
        if (enrollment.Status != "Active")
        {
            throw new InvalidOperationException(
                "Student must have an active enrollment.");
        }

        // 3. Student must complete the course
        var progress =
            await _courseProgressService
                .GetProgressAsync(
                    enrollment.EnrollmentId);

        if (progress == null ||
            !progress.IsCompleted)
        {
            throw new InvalidOperationException(
                "Student must complete the course before submitting feedback.");
        }

        // 4. Student can submit only one feedback
        var existingFeedback =
            await _feedbackRepository
                .GetByUserAndCourseAsync(
                    userId,
                    dto.CourseId);

        if (existingFeedback != null)
        {
            throw new InvalidOperationException(
                "Student has already submitted feedback for this course.");
        }

        // 5. Validate feedback
        ValidateFeedback(dto);

        // 6. Create feedback
        var feedback = new Feedback
        {
            Rating = dto.Rating,

            Comment =
                dto.Comment?.Trim()
                ?? string.Empty,

            Status = "Pending",

            CreatedAt =
                DateTime.UtcNow,

            UserId = userId,

            CourseId = dto.CourseId
        };

        await _feedbackRepository
            .AddAsync(feedback);

        await _feedbackRepository
            .SaveChangesAsync();

        return MapToDto(feedback);
    }

    public async Task UpdateAsync(
        int id,
        CreateFeedbackDto dto)
    {
        // 1. Get feedback
        var feedback =
            await _feedbackRepository
                .GetByIdAsync(id);

        if (feedback == null)
        {
            throw new KeyNotFoundException(
                "Feedback not found.");
        }

        // 2. Validate feedback
        ValidateFeedback(dto);

        // 3. Update feedback
        feedback.Rating =
            dto.Rating;

        feedback.Comment =
            dto.Comment?.Trim()
            ?? string.Empty;

        // Editing sends feedback
        // back for moderation.
        feedback.Status = "Pending";

        _feedbackRepository
            .Update(feedback);

        await _feedbackRepository
            .SaveChangesAsync();
    }

    public async Task DeleteAsync(
        int id)
    {
        var feedback =
            await _feedbackRepository
                .GetByIdAsync(id);

        if (feedback == null)
        {
            throw new KeyNotFoundException(
                "Feedback not found.");
        }

        _feedbackRepository
            .Delete(feedback);

        await _feedbackRepository
            .SaveChangesAsync();
    }

    private static void ValidateFeedback(
        CreateFeedbackDto dto)
    {
        bool hasRating =
            dto.Rating > 0;

        bool hasComment =
            !string.IsNullOrWhiteSpace(
                dto.Comment);

        // At least rating OR comment
        // is required.
        if (!hasRating &&
            !hasComment)
        {
            throw new InvalidOperationException(
                "Feedback must contain a rating or a comment.");
        }

        // Rating is optional.
        // If provided, it must be 1-5.
        if (dto.Rating < 0 ||
            dto.Rating > 5)
        {
            throw new ArgumentException(
                "Rating must be between 1 and 5.");
        }
    }

    private static FeedbackDto MapToDto(
        Feedback feedback)
    {
        return new FeedbackDto
        {
            FeedbackId =
                feedback.FeedbackId,

            Rating =
                feedback.Rating,

            Comment =
                feedback.Comment,

            Status =
                feedback.Status,

            CreatedAt =
                feedback.CreatedAt,

            UserId =
                feedback.UserId,

            CourseId =
                feedback.CourseId
        };
    }
}