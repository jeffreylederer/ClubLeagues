using System;
using System.ComponentModel.DataAnnotations;

namespace ClubLeagues.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "User Id")]
        public string EmailId { get; set; }
    }
}