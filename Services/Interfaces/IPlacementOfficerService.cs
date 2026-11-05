using CampusPlacementManagement.DTOs.PlacementOfficer;

namespace CampusPlacementManagement.Interfaces
{
    public interface IPlacementOfficerService
    {
        Task<PlacementOfficerResponseDto> RegisterAsync(
            PlacementOfficerRegisterDto dto);

        Task<List<PlacementOfficerResponseDto>> GetAllAsync();

        Task<PlacementOfficerResponseDto?> GetByIdAsync(int id);

        Task<List<PlacementOfficerResponseDto>> GetPendingAsync();

        Task<List<PlacementOfficerResponseDto>> GetByCollegeIdAsync(int collegeId);

        Task<bool> ApproveAsync(int id, string approvedBy);

        Task<bool> RejectAsync(int id);

        Task<bool> UpdateAsync(int id, PlacementOfficerUpdateDto dto);
    }
}