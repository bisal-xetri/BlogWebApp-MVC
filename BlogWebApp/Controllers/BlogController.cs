using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebApp.Data;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotyfService _notification;
        public BlogController(ApplicationDbContext context, INotyfService notyfyservice)
        {
            _context = context; 
            _notification = notyfyservice;
        }
        [HttpGet("[controller]/{slug}")]
        public async Task<IActionResult> Post(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                _notification.Error("Post not Found");
                return View();
            }

            var post = await _context.Posts.Include(x => x.ApplicationUser)
                                           .FirstOrDefaultAsync(x => x.Slug == slug);
            if (post == null)
            {
                _notification.Error("Post not Found");
                return View();
            }

            var vm = new BlogPostVM()
            {
                Id = post.Id,
                Title = post.Title,
                AuthorName = post.ApplicationUser!.FirstName + " " + post.ApplicationUser.LastName,
                CreatedDate = (DateTime)post.CreatedDate,
                Description = post.Description,
                ThumbnailUrl=post.ThumbnailUrl,
                ShortDescription= post.ShortDescription,
            };

            return View(vm);
        }

    }
}
