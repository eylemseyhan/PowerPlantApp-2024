using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PowerPlantApp.Context;
using PowerPlantApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace PowerPlantApp.Controllers
{
    public class AccessDeniedController : Controller
    {
        
        private readonly PowerPlantContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        
        public AccessDeniedController(PowerPlantContext context , UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        
        // email iletme metodu
        private async Task SetUserEmailInViewBag()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                ViewBag.UserEmail = user.Email;
            }
        }
        // GET: /AccessDenied
        public async Task <IActionResult> Index()
        {
            await SetUserEmailInViewBag();
            return View();
        }
    }
}