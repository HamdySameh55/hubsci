using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineLearning.Web.Controllers;

public class AboutController : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
}