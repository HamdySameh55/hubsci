using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface IFeedbackService
{
    Task<IEnumerable<FeedbackDto>> GetAllAsync(
        int userId);

    Task<IEnumerable<FeedbackDto>> GetAllForAdminAsync();

    Task<FeedbackDto?> GetByIdAsync(
        int id);

    Task<FeedbackDto> CreateAsync(
        CreateFeedbackDto dto,
        int userId);

    Task UpdateAsync(
        int id,
        CreateFeedbackDto dto);

    Task DeleteAsync(
        int id);
}