using BlogWebApp.Models;

namespace BlogWebApp.ViewModels
{
    public class HomeVM
    {
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ShortName { get; set; }
        public string? ThumbnailUrl { get; set; }
        public List<Post>? Posts { get; set; }

    }
}
