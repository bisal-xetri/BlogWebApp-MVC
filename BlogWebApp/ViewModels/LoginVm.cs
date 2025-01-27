using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels
{
    public class LoginVm
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        public bool RememberedMe { get; set; }

    }
}
