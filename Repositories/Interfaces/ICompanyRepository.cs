using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Repositories
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(int id);

        Task<Company?> GetByEmailAsync(string email);

        Task<List<Company>> GetAllAsync();

        Task<List<Company>> GetPendingAsync();

        Task AddAsync(Company company);

        void Update(Company company);

        Task SaveChangesAsync();
    }
}