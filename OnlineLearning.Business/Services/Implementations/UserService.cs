
using Microsoft.AspNetCore.Identity;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;

using OnlineLearning.Data.Repositories.Interfaces;

using OnlineLearning.Domain.Entities;

namespace OnlineLearning.Business.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly PasswordHasher<User> _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        PasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();

        return users.Select(user => new UserDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        });
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return null;

        return new UserDto
        {
            UserId = user.UserId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = dto.Role
        };

        user.PasswordHash =
            _passwordHasher.HashPassword(
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

    public async Task UpdateAsync(int id, CreateUserDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        user.Name = dto.Name;
        user.Email = dto.Email;
        user.Role = dto.Role;

        user.PasswordHash =
            _passwordHasher.HashPassword(
                user,
                dto.Password);

        _userRepository.Update(user);

        await _userRepository.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            throw new KeyNotFoundException("User not found.");

        // Admin users cannot be deleted.
        if (user.Role == "Admin")
            throw new InvalidOperationException(
                "Admin users cannot be deleted.");

        // User has enrollments.
        if (await _userRepository.HasEnrollmentsAsync(id))
            throw new InvalidOperationException(
                "This user cannot be deleted because they have enrollments.");

        // Instructor owns courses.
        if (await _userRepository.HasCoursesAsync(id))
            throw new InvalidOperationException(
                "This user cannot be deleted because they own courses.");

        // User has feedback.
        if (await _userRepository.HasFeedbacksAsync(id))
            throw new InvalidOperationException(
                "This user cannot be deleted because they have feedback.");

        _userRepository.Delete(user);

        await _userRepository.SaveChangesAsync();
    }
}
