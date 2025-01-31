using System.Diagnostics;
using BlogWebApp.Data;
using BlogWebApp.Models;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using Microsoft.EntityFrameworkCore;
using X.PagedList.Mvc.Core;
using X.PagedList.Extensions;

namespace BlogWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int? page)
        {
            var vm = new HomeVM();
            var setting = _context.Settings.ToList();
            vm.Title = setting[0].Title;
            vm.ShortDescription = setting[0].ShortDescription;
            vm.ThumbnailUrl = setting[0].ThumbnailUrl;

            int pageSize = 4;
            int pageNumber = page ?? 1;
            vm.Posts =  _context.Posts
                                    .Include(x => x.ApplicationUser)
                                    .OrderBy(x => x.CreatedDate) // Ensuring the query is ordered
                                    .ToPagedList(pageNumber, pageSize);

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
