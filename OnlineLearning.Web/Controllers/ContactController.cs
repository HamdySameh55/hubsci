using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineLearning.Web.Controllers;

public class ContactController : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
}