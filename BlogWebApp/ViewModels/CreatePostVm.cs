using System.ComponentModel.DataAnnotations;
using BlogWebApp.Models;

namespace BlogWebApp.ViewModels
{
    public class CreatePostVm
    {
        internal string? AppicationUserId;

        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? ShortDescription { get; set; }
        //Relation
        public string? ApplicationUserId { get; set; }

        public string? Description { get; set; }
    
        public string? ThumbnailUrl { get; set; }
        public IFormFile? Thumbnail { get; set; }
        public DateTime CreatedDate { get; set; }
     
    }
}
