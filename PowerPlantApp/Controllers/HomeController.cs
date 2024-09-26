using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class HomeController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public HomeController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        // e postayı navbarda göstermek için
        var user = await _userManager.GetUserAsync(User);
        ViewBag.UserEmail = user?.Email; 
        return View();
    }
}