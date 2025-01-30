using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebApp.Data;
using BlogWebApp.Models;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles ="Admin")]
    public class SettingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SettingController(ApplicationDbContext context, INotyfService notyfService, IWebHostEnvironment webHostEnvironment )
        {
            _context = context;
            _notification = notyfService;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var settings = _context.Settings.ToList();
            if( settings.Count > 0 )
            {
                var vm = new SettingVM()
                {
                    Id= settings[0].Id,
                    Title=settings[0].Title,
                    ThumbnailUrl = settings[0].ThumbnailUrl,
                    SiteName=settings[0].SiteName,
                    ShortDescription=settings[0].ShortDescription,
                    TwitterUrl=settings[0].TwitterUrl,
                    FacebookUrl=settings[0].FacebookUrl,
                    GitHubUrl=settings[0].GitHubUrl,
                };
                return View( vm );
            }
            var setting = new Setting()
            {
               
                SiteName = "Demo Name",
            };
            await _context.Settings.AddAsync(setting);
            await _context.SaveChangesAsync();
            var createdSetting = _context.Settings.ToList();
            var createdvm = new SettingVM()
            {
                Id = createdSetting[0].Id,
                Title = createdSetting[0].Title,
                ThumbnailUrl = createdSetting[0].ThumbnailUrl,
                SiteName = createdSetting[0].SiteName,
                ShortDescription = createdSetting[0].ShortDescription,
                TwitterUrl = createdSetting[0].TwitterUrl,
                FacebookUrl = createdSetting[0].FacebookUrl,
                GitHubUrl = createdSetting[0].GitHubUrl,
            };
            return View(createdvm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SettingVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            var setting= await _context.Settings.FirstOrDefaultAsync(x=>x.Id==vm.Id);
            if(setting == null)
            {
                _notification.Error("Something went Wrong");
                return View(vm);
            }
            setting.SiteName=vm.SiteName;
            setting.Title=vm.Title;
            setting.ShortDescription=vm.ShortDescription;
            setting.TwitterUrl=vm.TwitterUrl;
            setting.FacebookUrl=vm.FacebookUrl;
            setting.GitHubUrl=vm.GitHubUrl;
            if(vm.Thumbnail!=null)
            {
                setting.ThumbnailUrl=UploadImage(vm.Thumbnail);

            }
           
            await _context.SaveChangesAsync();
            _notification.Success("Setting Updated Successfully");
            return RedirectToAction("Index");
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
