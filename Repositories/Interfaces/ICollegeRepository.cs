
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Interfaces
{
    public interface ICollegeRepository
    {
        // Register a new college
        Task<College> AddAsync(College college);

        // Get all colleges
        Task<List<College>> GetAllAsync();

        // Get college by ID
        Task<College?> GetByIdAsync(int collegeId);

        // Get pending colleges
        Task<List<College>> GetPendingAsync();

        // Check whether college code already exists
        Task<bool> CollegeCodeExistsAsync(
            string collegeCode,
            int? excludeCollegeId = null);

        // Update college information
        Task UpdateAsync(College college);

        // Save database changes
        Task SaveChangesAsync();
    }
}
