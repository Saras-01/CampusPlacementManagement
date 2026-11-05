using CampusPlacementManagement.DTOs.Company;

namespace CampusPlacementManagement.Services
{
    public interface ICompanyService
    {
        Task<CompanyResponseDto> RegisterAsync(CompanyRegisterDto dto);

        Task<List<CompanyResponseDto>> GetAllAsync();

        Task<CompanyResponseDto?> GetByIdAsync(int id);

        Task<List<CompanyResponseDto>> GetPendingAsync();

        Task<bool> ApproveAsync(int id);

        Task<bool> RejectAsync(int id);

        Task<bool> SuspendAsync(int id);

        Task<bool> UpdateAsync(int id, CompanyUpdateDto dto);
    }
}