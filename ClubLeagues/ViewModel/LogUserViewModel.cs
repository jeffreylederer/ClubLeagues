
using System.ComponentModel.DataAnnotations;
namespace ClubLeagues.Models
{
    public class LogUserViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "User Id")]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}