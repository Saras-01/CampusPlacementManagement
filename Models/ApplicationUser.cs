using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(150)]
        public string FullName { get; set; } = string.Empty;

        public PlacementOfficer? PlacementOfficer { get; set; }

        public ICollection<PlacementOfficer> ApprovedPlacementOfficers
        { get; set; } = new List<PlacementOfficer>();
    }
}