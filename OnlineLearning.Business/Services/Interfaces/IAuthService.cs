using OnlineLearning.Business.DTOs;
using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Interfaces;

public interface IAuthService
{
Task<User?> LoginAsync(LoginDto dto);

Task<User?> GetByEmailAsync(string email);

Task<UserDto> RegisterAsync(RegisterDto dto);

Task<UserDto> RegisterInstructorAsync(RegisterDto dto);


}
