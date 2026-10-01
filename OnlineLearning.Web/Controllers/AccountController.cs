using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;

namespace OnlineLearning.Web.Controllers;

public class AccountController : Controller
{
private readonly IAuthService _authService;

public AccountController(IAuthService authService)
{
    _authService = authService;
}


// =====================================================
// Login
// =====================================================

[HttpGet]
public IActionResult Login()
{
    return View();
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Login(LoginDto dto)
{
    if (!ModelState.IsValid)
    {
        return View(dto);
    }

    var user = await _authService.LoginAsync(dto);

    if (user == null)
    {
        ModelState.AddModelError(
            "",
            "Invalid email or password.");

        return View(dto);
    }

    var claims = new List<Claim>
    {
        new Claim(
            ClaimTypes.NameIdentifier,
            user.UserId.ToString()),

        new Claim(
            ClaimTypes.Name,
            user.Name),

        new Claim(
            ClaimTypes.Email,
            user.Email),

        new Claim(
            ClaimTypes.Role,
            user.Role)
    };

    var identity = new ClaimsIdentity(
        claims,
        CookieAuthenticationDefaults.AuthenticationScheme);

    var principal = new ClaimsPrincipal(identity);

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal);

    return RedirectToAction(
        "Index",
        "Home");
}


// =====================================================
// Register Student
// =====================================================

[HttpGet]
public IActionResult Register()
{
    return View();
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Register(RegisterDto dto)
{
    if (!ModelState.IsValid)
    {
        return View(dto);
    }

    var existingUser =
        await _authService.GetByEmailAsync(dto.Email);

    if (existingUser != null)
    {
        ModelState.AddModelError(
            "Email",
            "Email is already registered.");

        return View(dto);
    }

    await _authService.RegisterAsync(dto);

    return RedirectToAction(nameof(Login));
}


// =====================================================
// Register Instructor
// =====================================================

[HttpGet]
public IActionResult InstructorRegister()
{
    return View();
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> InstructorRegister(
    RegisterDto dto)
{
    if (!ModelState.IsValid)
    {
        return View(dto);
    }

    var existingUser =
        await _authService.GetByEmailAsync(dto.Email);

    if (existingUser != null)
    {
        ModelState.AddModelError(
            "Email",
            "Email is already registered.");

        return View(dto);
    }

    await _authService.RegisterInstructorAsync(dto);

    return RedirectToAction(nameof(Login));
}


// =====================================================
// Logout
// =====================================================

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Logout()
{
    await HttpContext.SignOutAsync(
        CookieAuthenticationDefaults.AuthenticationScheme);

    return RedirectToAction(nameof(Login));
}


// =====================================================
// Access Denied
// =====================================================

[HttpGet]
public IActionResult AccessDenied()
{
    return View();
}

}
