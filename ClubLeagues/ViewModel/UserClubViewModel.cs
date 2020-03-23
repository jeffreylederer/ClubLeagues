using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ClubLeagues.Models
{
    public class UserClubViewModel
    {
        [Key]
        public int clubid { get; set; }
        public string Name { get; set; }
        public IEnumerable<UserClub> userClubs { get; set; }
    }
}