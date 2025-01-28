using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.ViewModels
{
    public class ResetPasword
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        [Required]
        public string? NewPassword{ get; set; }
        [Compare(nameof(NewPassword))]
        [Required]
        public string? ConfirmPassword { get; set; }
    }
}
