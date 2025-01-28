using BlogWebApp.Models;

namespace BlogWebApp.ViewModels
{
    public class CreatePostVm
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        //Relation
     
        public string? Description { get; set; }
    
        public string? ThumbnailUrl { get; set; }
        public IFormFile? Thumbnail { get; set; }

    }
}
