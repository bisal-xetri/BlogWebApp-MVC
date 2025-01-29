using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebApp.Data;
using BlogWebApp.Models;
using BlogWebApp.Utilities;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        public INotyfService _notification;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(ApplicationDbContext context, INotyfService notyfService, IWebHostEnvironment webHostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _notification = notyfService;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var listOfPosts = new List<Post>();
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

            if (loggedInUserRole[0] == WebSiteRoles.WebsiteAdmin)
            {
                listOfPosts = await _context.Posts.Include(x => x.ApplicationUser).ToListAsync();
            }
            else
            {
                listOfPosts = await _context.Posts.Include(x => x.ApplicationUser)
                                                  .Where(x => x.ApplicationUser!.Id == loggedInUser.Id)
                                                  .ToListAsync();
            }

            var listOfPostVm = listOfPosts.Select(x => new PostVM()
            {
                Id = x.Id,
                Title = x.Title,
                CreatedDate = (DateTime)x.CreatedDate,
                ThumbnailUrl = x.ThumbnailUrl,
                AuthorName = x.ApplicationUser!.FirstName + " " + x.ApplicationUser.LastName
            }).ToList();

            return View(listOfPostVm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreatePostVm());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePostVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var post = new Post
            {
                Title = vm.Title,
                Description = vm.Description,
                ShortDescription = vm.ShortDescription,
                ApplicationUserId = loggedInUser!.Id
            };

            if (post.Title != null)
            {
                string slug = vm.Title!.Trim().Replace(" ", "-");
                post.Slug = slug + "-" + Guid.NewGuid();
            }

            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            _notification.Success("Post Created Successfully");
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

            if (loggedInUserRole[0] == WebSiteRoles.WebsiteAdmin && loggedInUser!.Id == post!.ApplicationUserId)
            {
                _context.Posts.Remove(post!);
                await _context.SaveChangesAsync();
                _notification.Success("Post Deleted Successfully");
                return RedirectToAction("Index", "Post", new { area = "Admin" });
            }

            _notification.Error("You do not have permission to delete this post.");
            return RedirectToAction("Index", "Post", new { area = "Admin" });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == id);
            if (post == null)
            {
                _notification.Error("Post not found.");
                return RedirectToAction("Index");
            }
            var loggedInUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == User.Identity!.Name);
            var loggedInUserRole = await _userManager.GetRolesAsync(loggedInUser!);

            if (loggedInUserRole[0] != WebSiteRoles.WebsiteAdmin || loggedInUser!.Id!= post.ApplicationUserId)
            {
                _notification.Error("You are Not Authorize.");
                return RedirectToAction("Index");
            }
            var vm = new CreatePostVm()
            {
                Id = post.Id,
                Title = post.Title,
                ShortDescription = post.ShortDescription,
                ApplicationUserId = post.ApplicationUserId,
                Description = post.Description,
                ThumbnailUrl = post.ThumbnailUrl,
                CreatedDate = (DateTime)post.CreatedDate
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CreatePostVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var post = await _context.Posts.FirstOrDefaultAsync(x => x.Id == vm.Id);
            if (post == null)
            {
                _notification.Error("Post not found.");
                return View(vm);
            }
            post.Id = vm.Id;
            post.Title = vm.Title;
            post.Description = vm.Description;
            post.ShortDescription = vm.ShortDescription;
          

            if (vm.Thumbnail != null)
            {
                post.ThumbnailUrl = UploadImage(vm.Thumbnail);
            }

            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
            _notification.Success("Post Updated Successfully.");
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
