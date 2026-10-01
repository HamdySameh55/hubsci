using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface IAdminDashboardService
{
    Task<AdminDashboardDto> GetDashboardAsync();
}