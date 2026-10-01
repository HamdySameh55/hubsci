using Microsoft.AspNetCore.Identity;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;

using OnlineLearning.Data.Repositories.Interfaces;

using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class AuthService : IAuthService
{
private readonly IUserRepository _userRepository;
private readonly PasswordHasher<User> _passwordHasher;

public AuthService(
    IUserRepository userRepository,
    PasswordHasher<User> passwordHasher)
{
    _userRepository = userRepository;
    _passwordHasher = passwordHasher;
}


// =====================================================
// Login
// =====================================================

public async Task<User?> LoginAsync(LoginDto dto)
{
    var user = await _userRepository.GetByEmailAsync(dto.Email);

    if (user == null)
    {
        return null;
    }

    var result = _passwordHasher.VerifyHashedPassword(
        user,
        user.PasswordHash,
        dto.Password);

    if (result == PasswordVerificationResult.Failed)
    {
        return null;
    }

    return user;
}


// =====================================================
// Get User By Email
// =====================================================

public async Task<User?> GetByEmailAsync(string email)
{
    return await _userRepository.GetByEmailAsync(email);
}


// =====================================================
// Register Student
// =====================================================

public async Task<UserDto> RegisterAsync(RegisterDto dto)
{
    var user = new User
    {
        Name = dto.Name,
        Email = dto.Email,
        Role = "Student"
    };

    user.PasswordHash = _passwordHasher.HashPassword(
        user,
        dto.Password);

    await _userRepository.AddAsync(user);

    await _userRepository.SaveChangesAsync();

    return new UserDto
    {
        UserId = user.UserId,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role
    };
}


// =====================================================
// Register Instructor
// =====================================================

public async Task<UserDto> RegisterInstructorAsync(
    RegisterDto dto)
{
    var user = new User
    {
        Name = dto.Name,
        Email = dto.Email,
        Role = "Instructor"
    };

    user.PasswordHash = _passwordHasher.HashPassword(
        user,
        dto.Password);

    await _userRepository.AddAsync(user);

    await _userRepository.SaveChangesAsync();

    return new UserDto
    {
        UserId = user.UserId,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role
    };
}


}
