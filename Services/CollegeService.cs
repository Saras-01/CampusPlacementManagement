
using CampusPlacementManagement.DTOs.College;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;

namespace CampusPlacementManagement.Services
{
    public class CollegeService : ICollegeService
    {
        private readonly ICollegeRepository _collegeRepository;

        public CollegeService(ICollegeRepository collegeRepository)
        {
            _collegeRepository = collegeRepository;
        }

        // 1. Register College
        public async Task<CollegeResponseDto> RegisterCollegeAsync(
            CollegeRegisterDto dto)
        {
            // Check duplicate college code
            if (!string.IsNullOrWhiteSpace(dto.CollegeCode))
            {
                bool exists = await _collegeRepository
                    .CollegeCodeExistsAsync(dto.CollegeCode.Trim());

                if (exists)
                {
                    throw new InvalidOperationException(
                        "College code already exists.");
                }
            }

            var college = new College
            {
                CollegeName = dto.CollegeName.Trim(),
                CollegeCode = string.IsNullOrWhiteSpace(dto.CollegeCode)
                    ? null : dto.CollegeCode.Trim(),
                UniversityName = dto.UniversityName?.Trim(),
                OfficialEmail = dto.OfficialEmail.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                Address = dto.Address.Trim(),
                Website = dto.Website?.Trim(),
                Status = CollegeStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _collegeRepository.AddAsync(college);
            await _collegeRepository.SaveChangesAsync();

            return MapToResponse(college);
        }

        // 2. Get All Colleges
        public async Task<List<CollegeResponseDto>> GetAllCollegesAsync()
        {
            var colleges = await _collegeRepository.GetAllAsync();

            return colleges.Select(MapToResponse).ToList();
        }

        // 3. Get College By ID
        public async Task<CollegeResponseDto?> GetCollegeByIdAsync(
            int collegeId)
        {
            var college = await _collegeRepository.GetByIdAsync(collegeId);

            if (college == null)
                return null;

            return MapToResponse(college);
        }

        // 4. Get Pending Colleges
        public async Task<List<CollegeResponseDto>> GetPendingCollegesAsync()
        {
            var colleges = await _collegeRepository.GetPendingAsync();

            return colleges.Select(MapToResponse).ToList();
        }

        // 5. Approve College
        public async Task<bool> ApproveCollegeAsync(int collegeId)
        {
            var college = await _collegeRepository.GetByIdAsync(collegeId);

            if (college == null)
                return false;

            if (college.Status != CollegeStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending colleges can be approved.");

            college.Status = CollegeStatus.Approved;

            await _collegeRepository.SaveChangesAsync();

            return true;
        }

        // 6. Reject College
        public async Task<bool> RejectCollegeAsync(int collegeId)
        {
            var college = await _collegeRepository.GetByIdAsync(collegeId);

            if (college == null)
                return false;

            if (college.Status != CollegeStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending colleges can be rejected.");

            college.Status = CollegeStatus.Rejected;

            await _collegeRepository.SaveChangesAsync();

            return true;
        }
        // 7. Suspend College
        public async Task<bool> SuspendCollegeAsync(int collegeId)
        {
            var college = await _collegeRepository.GetByIdAsync(collegeId);

            if (college == null)
                return false;

            if (college.Status != CollegeStatus.Approved)
                throw new InvalidOperationException(
                    "Only approved colleges can be suspended.");

            college.Status = CollegeStatus.Suspended;

            await _collegeRepository.SaveChangesAsync();

            return true;
        }

        // 8. Update College
        public async Task<bool> UpdateCollegeAsync(
            int collegeId,
            CollegeUpdateDto dto)
        {
            var college = await _collegeRepository.GetByIdAsync(collegeId);

            if (college == null)
                return false;

            // Check duplicate college code
            if (!string.IsNullOrWhiteSpace(dto.CollegeCode))
            {
                bool exists = await _collegeRepository
                    .CollegeCodeExistsAsync(
                        dto.CollegeCode.Trim(),
                        collegeId);

                if (exists)
                {
                    throw new InvalidOperationException(
                        "College code already exists.");
                }
            }

            college.CollegeName = dto.CollegeName.Trim();

            college.CollegeCode =
                string.IsNullOrWhiteSpace(dto.CollegeCode)
                ? null : dto.CollegeCode.Trim();

            college.UniversityName = dto.UniversityName?.Trim();
            college.OfficialEmail = dto.OfficialEmail.Trim();
            college.PhoneNumber = dto.PhoneNumber.Trim();
            college.Address = dto.Address.Trim();
            college.Website = dto.Website?.Trim();

            await _collegeRepository.UpdateAsync(college);
            await _collegeRepository.SaveChangesAsync();

            return true;
        }

        // Convert Entity to Response DTO
        private static CollegeResponseDto MapToResponse(College college)
        {
            return new CollegeResponseDto
            {
                CollegeId = college.CollegeId,
                CollegeName = college.CollegeName,
                CollegeCode = college.CollegeCode,
                UniversityName = college.UniversityName,
                OfficialEmail = college.OfficialEmail,
                PhoneNumber = college.PhoneNumber,
                Address = college.Address,
                Website = college.Website,
                Status = college.Status,
                CreatedAt = college.CreatedAt
            };
        }
    }
}
