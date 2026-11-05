using System.ComponentModel.DataAnnotations;
using CampusPlacementManagement.Enums;

namespace CampusPlacementManagement.Models
{
    public class PlacementOfficer
    {
        [Key]
        public int PlacementOfficerId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CollegeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Designation { get; set; } = string.Empty;

        [Required]
        public AccountStatus Status { get; set; } = AccountStatus.Pending;

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovedAt { get; set; }

        // Navigation properties

        public ApplicationUser User { get; set; } = null!;

        public College College { get; set; } = null!;

        public ApplicationUser? Approver { get; set; }
    }
}