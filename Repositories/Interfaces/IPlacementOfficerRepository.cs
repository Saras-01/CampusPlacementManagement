using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Interfaces
{
    public interface IPlacementOfficerRepository
    {
        Task AddAsync(PlacementOfficer placementOfficer);

        Task<PlacementOfficer?> GetByIdAsync(int id);

        Task<PlacementOfficer?> GetByUserIdAsync(string userId);

        Task<List<PlacementOfficer>> GetAllAsync();

        Task<List<PlacementOfficer>> GetPendingAsync();

        Task<List<PlacementOfficer>> GetByCollegeIdAsync(int collegeId);

        void Update(PlacementOfficer placementOfficer);

        Task SaveChangesAsync();
    }
}