using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using OnlineLearning.Business.DTOs;
using OnlineLearning.Business.Services.Interfaces;

namespace OnlineLearning.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: /User
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllAsync();

        return View(users);
    }

    // GET: /User/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /User/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await _userService.CreateAsync(dto);

        return RedirectToAction(nameof(Index));
    }

    // POST: /User/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _userService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            TempData["DeleteError"] = ex.Message;

            return RedirectToAction(nameof(Index));
        }
    }
}