﻿using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebApp.Models;
using BlogWebApp.Utilities;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.ComponentModel;
using System.Net.WebSockets;
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
                UserName = x.UserName,
                Email = x.Email,

            }).ToList();
            foreach(var user in vm)
            {
                var singleUser = await _userManager.FindByIdAsync(user.Id);
                var role = await _userManager.GetRolesAsync(singleUser);
                user.Roles = role.FirstOrDefault();
            }
            
            return View(vm);
        }
        //reset password
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var exitingUsser = await _userManager.FindByIdAsync(id);
            if(exitingUsser == null)
            {
                _notification.Error("User Dosenot Exits.");
                return View();
            }
            var vm = new ResetPasword()
            {
                Id = exitingUsser.Id,
                UserName = exitingUsser.UserName

            };
            return View(vm);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasword vm)
        {
            if (!ModelState.IsValid)
            {
              
                return View(vm);
            }
           
            var exitingUser = await _userManager.FindByIdAsync(vm.Id);
            if(exitingUser == null)
            {
                _notification.Error("User Dosenot Exits");
                return View(vm);
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(exitingUser);
            var result=await _userManager.ResetPasswordAsync(exitingUser,token, vm.NewPassword);
            if (result.Succeeded)
            {
                _notification.Success("Password Reset Successful");
                return RedirectToAction(nameof(Index));
            }
           return View(vm);

        }

        [Authorize(Roles="Admin")]
        [HttpGet]
        public  IActionResult Register()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVm vm)
        {
            if (!ModelState.IsValid)
            {
               return View(vm);
            }
            var checkUserByEmail=await _userManager.FindByEmailAsync(vm.Email);
            if (checkUserByEmail!=null)
            {
                _notification.Error("Email Already Exits.");
                return View(vm);
            }
            var checkUserByUserName= await _userManager.FindByNameAsync(vm.UserName);
            if (checkUserByUserName!=null)
            {
                _notification.Error("UserName Already Exits.");
                return View(vm);
            }
            var user = new ApplicationUser()
            {
                Email = vm.Email,
                UserName = vm.UserName,
                FirstName = vm.FirstName,
                LastName = vm.LastName,
            };
           var checkUser= await _userManager.CreateAsync(user,vm.Password);
            if (checkUser.Succeeded) {
                if (vm.IsAdmin)
                {
                    await _userManager.AddToRoleAsync(user, WebSiteRoles.WebsiteAdmin);
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, WebSiteRoles.WebsiteAuthor);
                }
                _notification.Success("User Register Successfully");
               return RedirectToAction("Index", "User", new {area="Admin"});
            }
            return View(vm);
        }

        [HttpGet("Login")]
        public IActionResult Login()
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return View(new LoginVm());
            }
            return RedirectToAction("Index","Post",new {area="Admin"});
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
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }

        [HttpPost]
        [Authorize]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            _notification.Success("Logout Successful");
            return RedirectToAction("Index", "Home", new {area=""});
        }
        [HttpGet("AccessDenied")]
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
