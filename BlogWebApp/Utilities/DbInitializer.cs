using BlogWebApp.Data;
using BlogWebApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogWebApp.Utilities
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void Initialize()
        {
            if (!_roleManager.RoleExistsAsync(WebSiteRoles.WebsiteAdmin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(WebSiteRoles.WebsiteAdmin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(WebSiteRoles.WebsiteAuthor)).GetAwaiter().GetResult();
                _userManager.CreateAsync(new ApplicationUser()
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    FirstName = "Super",
                    LastName = "Admin"
                }, "Admin@001").Wait();
            }
            var appUser = _context.ApplicationUsers.FirstOrDefault(x => x.Email == "admin@gmail.com");
            if (appUser != null)
            {
                _userManager.AddToRoleAsync(appUser, WebSiteRoles.WebsiteAdmin).GetAwaiter().GetResult();
            }

            var aboutPage = new Page()
            {
                Title = "About Us",
                Slug = "about"
            };
            var contactPage = new Page()
            {
                Title = "Contact Us",
                Slug = "contact"
            };
            var privacyPage = new Page()
            {
                Title = "Privacy Policy",
                Slug = "privacy"
            };
            _context.Pages.Add(aboutPage);
            _context.Pages.Add(contactPage);
            _context.Pages.Add(privacyPage);
            _context.SaveChanges();
            //alternative way of doing above code

            //var listOfPages = new List<Page>()
            //{
            //    new Page(){
            //    Title="About Us",
            //    Slug="about"
            //    },
            //     new Page(){
            //    Title="Contact Us",
            //    Slug="contact"
            //    },
            //      new Page(){
            //    Title="Privacy Policy",
            //    Slug="privacy"
            //    }
            //};
            //_context.Pages.AddRange(listOfPages);  
            //_context.SaveChanges();
        }

        public void Initialzer()
        {
           throw new NotImplementedException();
        }
    }
}
