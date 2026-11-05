
using CampusPlacementManagement.DTOs.College;

namespace CampusPlacementManagement.Interfaces
{
    public interface ICollegeService
    {
        // Register college
        Task<CollegeResponseDto> RegisterCollegeAsync(
            CollegeRegisterDto dto);

        // Get all colleges
        Task<List<CollegeResponseDto>> GetAllCollegesAsync();

        // Get college by ID
        Task<CollegeResponseDto?> GetCollegeByIdAsync(
            int collegeId);

        // Get pending colleges
        Task<List<CollegeResponseDto>> GetPendingCollegesAsync();

        // Approve college
        Task<bool> ApproveCollegeAsync(int collegeId);

        // Reject college
        Task<bool> RejectCollegeAsync(int collegeId);

        // Suspend college
        Task<bool> SuspendCollegeAsync(int collegeId);

        // Update college
        Task<bool> UpdateCollegeAsync(
            int collegeId,
            CollegeUpdateDto dto);
    }
}
