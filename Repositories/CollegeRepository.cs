
using CampusPlacementManagement.Data;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Enums;
using Microsoft.EntityFrameworkCore;

namespace CampusPlacementManagement.Repositories
{
    public class CollegeRepository : ICollegeRepository
    {
        private readonly ApplicationDbContext _context;

        public CollegeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<College> AddAsync(College college)
        {
            await _context.Colleges.AddAsync(college);

            return college;
        }

        public async Task<List<College>> GetAllAsync()
        {
            return await _context.Colleges
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<College?> GetByIdAsync(int collegeId)
        {
            return await _context.Colleges
                .FirstOrDefaultAsync(c => c.CollegeId == collegeId);
        }

        public async Task<List<College>> GetPendingAsync()
        {
            return await _context.Colleges
                .AsNoTracking()
                .Where(c => c.Status == CollegeStatus.Pending)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> CollegeCodeExistsAsync(
            string collegeCode,
            int? excludeCollegeId = null)
        {
            var query = _context.Colleges
                .AsQueryable();

            if (excludeCollegeId.HasValue)
            {
                query = query.Where(
                    c => c.CollegeId != excludeCollegeId.Value);
            }

            return await query.AnyAsync(
                c => c.CollegeCode == collegeCode);
        }

        public Task UpdateAsync(College college)
        {
            _context.Colleges.Update(college);

            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
