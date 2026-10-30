using Microsoft.AspNetCore.Mvc;

namespace KarateClubManagement.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Error()
    {
        return View();
    }
}
