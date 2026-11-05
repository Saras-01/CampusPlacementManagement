using CampusPlacementManagement.DTOs.Auth;

namespace CampusPlacementManagement.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    }
}