using System.Runtime.InteropServices;
using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebApp.Data;
using BlogWebApp.Models;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class PageController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public PageController(ApplicationDbContext context, INotyfService notyfService, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _notification = notyfService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]

        public async Task<IActionResult> About()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "about");
            var vm = new PageVM() { 
                Id=page!.Id,
                Title=page.Title,
                ShortDescription=page.ShortDescription,
                Description=page.Description,
                ThumbnailUrl=page.ThumbnailUrl,
                
            };
        return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> About(PageVM vm)

        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "about");
                if(page == null)
            {
                _notification.Error("About page not Found! ");
                return View();
            }
                page.Title = vm.Title;
               page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;
            if(vm.ThumbnailUrl != null)
            {
                page.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
           await _context.SaveChangesAsync();
            _notification.Success("About Page Updated Successfully");
            return RedirectToAction("About", "Page", new {area="Admin"});
        }
        [HttpGet]

        public async Task<IActionResult> Contact()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "contact");
            var vm = new PageVM()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl,

            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Contact(PageVM vm)

        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "contact");
            if (page == null)
            {
                _notification.Error("Contact page not Found! ");
                return View();
            }
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;
            if (vm.ThumbnailUrl != null)
            {
                page.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Contact Page Updated Successfully");
            return RedirectToAction("Contact", "Page", new { area = "Admin" });
        }

        [HttpGet]

        public async Task<IActionResult> Privacy()
        {
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "privacy");
            var vm = new PageVM()
            {
                Id = page!.Id,
                Title = page.Title,
                ShortDescription = page.ShortDescription,
                Description = page.Description,
                ThumbnailUrl = page.ThumbnailUrl,

            };
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Privacy(PageVM vm)

        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var page = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == "privacy");
            if (page == null)
            {
                _notification.Error("Privacy page not Found! ");
                return View();
            }
            page.Title = vm.Title;
            page.ShortDescription = vm.ShortDescription;
            page.Description = vm.Description;
            if (vm.ThumbnailUrl != null)
            {
                page.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }
            await _context.SaveChangesAsync();
            _notification.Success("Privacy Page Updated Successfully");
            return RedirectToAction("Privacy", "Page", new { area = "Admin" });
        }
        private string UploadImage(IFormFile file)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "Thumbnail");
            var filePath = Path.Combine(folderPath, uniqueFileName);

            using (var fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }

            return uniqueFileName;
        }
    }
}
