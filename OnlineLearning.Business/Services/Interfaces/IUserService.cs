using OnlineLearning.Business.DTOs;

namespace OnlineLearning.Business.Services.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();

    Task<UserDto?> GetByIdAsync(int id);

    Task<UserDto> CreateAsync(CreateUserDto dto);

    Task UpdateAsync(int id, CreateUserDto dto);

    Task DeleteAsync(int id);
}