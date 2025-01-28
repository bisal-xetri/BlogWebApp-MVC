using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebApp.Models;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlogWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notification;

        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, INotyfService notification)
        {
            _notification = notification;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users=await _userManager.Users.ToListAsync();
            var vm = users.Select(x => new UserVm()
            {
                Id = x.Id,
                FirstName=x.FirstName,
                LastName=x.LastName,
                UserName = x.UserName
            }).ToList();
            
            return View(vm);
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return View(new LoginVm());
            }
            return RedirectToAction("Index","User",new {area="Admin"});
        }

        public UserManager<ApplicationUser> Get_userManager()
        {
            return _userManager;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var checkUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == vm.Username);
            if (checkUser == null)
            {
                _notification.Error("Username does not exist");
                return View(vm);
            }
            var verifyPassword = await _userManager.CheckPasswordAsync(checkUser, vm.Password);
            if (!verifyPassword)
            {
                _notification.Error("Password does not match!!");
                return View(vm);
            }
            await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberedMe, true);
            _notification.Success("Login Successful");
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        [HttpPost]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _notification.Success("Logout Successful");
            return RedirectToAction("Index", "Home", new {area=""});
        }
    }
}
