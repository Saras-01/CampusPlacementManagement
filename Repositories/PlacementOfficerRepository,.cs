using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;

using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories
{
    public class PlacementOfficerRepository : IPlacementOfficerRepository
    {
        private readonly ApplicationDbContext _context;

        public PlacementOfficerRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add Placement Officer
        public async Task AddAsync(PlacementOfficer placementOfficer)
        {
            await _context.PlacementOfficers.AddAsync(placementOfficer);
        }

        // Get Placement Officer by ID
        public async Task<PlacementOfficer?> GetByIdAsync(int id)
        {
            return await _context.PlacementOfficers
                .Include(po => po.User)
                .Include(po => po.College)
                .Include(po => po.Approver)
                .FirstOrDefaultAsync(po => po.PlacementOfficerId == id);
        }

        // Get Placement Officer by User ID
        public async Task<PlacementOfficer?> GetByUserIdAsync(string userId)
        {
            return await _context.PlacementOfficers
                .Include(po => po.User)
                .Include(po => po.College)
                .FirstOrDefaultAsync(po => po.UserId == userId);
        }

        // Get all Placement Officers
        public async Task<List<PlacementOfficer>> GetAllAsync()
        {
            return await _context.PlacementOfficers
                .Include(po => po.User)
                .Include(po => po.College)
                .Include(po => po.Approver)
                .ToListAsync();
        }

        // Get pending Placement Officers
        public async Task<List<PlacementOfficer>> GetPendingAsync()
        {
            return await _context.PlacementOfficers
                .Include(po => po.User)
                .Include(po => po.College)
                .Where(po => po.Status == CampusPlacementManagement.Enums.AccountStatus.Pending)
                .ToListAsync();
        }

        // Get Placement Officers by College
        public async Task<List<PlacementOfficer>> GetByCollegeIdAsync(int collegeId)
        {
            return await _context.PlacementOfficers
                .Include(po => po.User)
                .Include(po => po.College)
                .Where(po => po.CollegeId == collegeId)
                .ToListAsync();
        }

        // Update Placement Officer
        public void Update(PlacementOfficer placementOfficer)
        {
            _context.PlacementOfficers.Update(placementOfficer);
        }

        // Save changes
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}