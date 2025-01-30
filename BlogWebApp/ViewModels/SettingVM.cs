using System.ComponentModel.DataAnnotations.Schema;

namespace BlogWebApp.ViewModels
{
    public class SettingVM
    {
        public int Id { get; set; }
        public string? SiteName { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? TwitterUrl { get; set; }
       
        public IFormFile? Thumbnail { get; set; }
    }
}
