using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.PlacementOfficer;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Models;

using Microsoft.AspNetCore.Identity;

namespace CampusPlacementManagement.Services
{
    public class PlacementOfficerService : IPlacementOfficerService
    {
        private readonly IPlacementOfficerRepository _repository;
        private readonly ICollegeRepository _collegeRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public PlacementOfficerService(
            IPlacementOfficerRepository repository,
            ICollegeRepository collegeRepository,
            UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _collegeRepository = collegeRepository;
            _userManager = userManager;
        }

        // Register Placement Officer
        public async Task<PlacementOfficerResponseDto> RegisterAsync(
    PlacementOfficerRegisterDto dto)
        {
            // 1. Check whether college exists
            var college = await _collegeRepository.GetByIdAsync(dto.CollegeId);

            if (college == null)
                throw new InvalidOperationException("College not found.");

            if (college.Status != CollegeStatus.Approved)
                throw new InvalidOperationException(
                    "Only approved colleges can register Placement Officers.");

            // 2. Check whether an officer profile already exists
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);

            ApplicationUser user;
            bool isNewUser = false;

            if (existingUser != null)
            {
                var existingOfficer =
                    await _repository.GetByUserIdAsync(existingUser.Id);

                if (existingOfficer != null)
                    throw new InvalidOperationException(
                        "This user is already registered as a Placement Officer.");

                if (await _userManager.IsInRoleAsync(
                    existingUser, UserRoles.SuperAdmin))
                {
                    throw new InvalidOperationException(
                        "This email belongs to a SuperAdmin account.");
                }

                // Reuse the existing Identity account
                user = existingUser;
            }
            else
            {
                // 3. Create a new Identity account only if none exists
                user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName
                };

                var createUserResult =
                    await _userManager.CreateAsync(user, dto.Password);

                if (!createUserResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        createUserResult.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(errors);
                }

                isNewUser = true;
            }

            // 4. Create Placement Officer profile
            var officer = new PlacementOfficer
            {
                UserId = user.Id,
                CollegeId = dto.CollegeId,
                Designation = dto.Designation,
                Status = AccountStatus.Pending
            };

            try
            {
                await _repository.AddAsync(officer);
                await _repository.SaveChangesAsync();
            }
            catch
            {
                // Delete only accounts created during this attempt.
                if (isNewUser)
                {
                    await _userManager.DeleteAsync(user);
                }

                throw;
            }

            return MapToResponse(officer, user, college);
        }

        // Get all Placement Officers
        public async Task<List<PlacementOfficerResponseDto>> GetAllAsync()
        {
            var officers = await _repository.GetAllAsync();

            return officers.Select(po =>
                MapToResponse(po, po.User!, po.College!)).ToList();
        }

        // Get Placement Officer by ID
        public async Task<PlacementOfficerResponseDto?> GetByIdAsync(int id)
        {
            var officer = await _repository.GetByIdAsync(id);

            if (officer == null)
                return null;

            return MapToResponse(
                officer, officer.User!, officer.College!);
        }

        // Get pending Placement Officers
        public async Task<List<PlacementOfficerResponseDto>> GetPendingAsync()
        {
            var officers = await _repository.GetPendingAsync();

            return officers.Select(po =>
                MapToResponse(po, po.User!, po.College!)).ToList();
        }

        // Get officers by College
        public async Task<List<PlacementOfficerResponseDto>> GetByCollegeIdAsync(
            int collegeId)
        {
            var officers = await _repository.GetByCollegeIdAsync(collegeId);

            return officers.Select(po =>
                MapToResponse(po, po.User!, po.College!)).ToList();
        }

        // Approve Placement Officer
        public async Task<bool> ApproveAsync(int id, string approvedBy)
        {
            var officer = await _repository.GetByIdAsync(id);

            if (officer == null)
                return false;

            if (officer.Status != AccountStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending Placement Officers can be approved.");

            officer.Status = AccountStatus.Approved;
            officer.ApprovedBy = approvedBy;
            officer.ApprovedAt = DateTime.UtcNow;

            var user = await _userManager.FindByIdAsync(officer.UserId);

            if (user == null)
                throw new InvalidOperationException(
                    "Placement Officer user account not found.");

            var roleResult = await _userManager.AddToRoleAsync(
                user, UserRoles.PlacementOfficer);

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    "; ", roleResult.Errors.Select(e => e.Description));

                throw new InvalidOperationException(errors);
            }

            _repository.Update(officer);
            await _repository.SaveChangesAsync();

            return true;
        }

        // Reject Placement Officer
        public async Task<bool> RejectAsync(int id)
        {
            var officer = await _repository.GetByIdAsync(id);

            if (officer == null)
                return false;

            if (officer.Status != AccountStatus.Pending)
                throw new InvalidOperationException(
                    "Only pending Placement Officers can be rejected.");

            officer.Status = AccountStatus.Rejected;

            _repository.Update(officer);
            await _repository.SaveChangesAsync();

            return true;
        }
        // Suspend Placement Officer
        public async Task<bool> SuspendAsync(int id)
        {
            var officer = await _repository.GetByIdAsync(id);

            if (officer == null)
                return false;

            if (officer.Status != AccountStatus.Approved)
                throw new InvalidOperationException(
                    "Only approved Placement Officers can be suspended.");

            officer.Status = AccountStatus.Suspended;

            _repository.Update(officer);

            await _repository.SaveChangesAsync();

            return true;
        }

        // Update Placement Officer
        public async Task<bool> UpdateAsync(
            int id, PlacementOfficerUpdateDto dto)
        {
            var officer = await _repository.GetByIdAsync(id);

            if (officer == null)
                return false;

            officer.Designation = dto.Designation;

            if (officer.User != null)
                officer.User.FullName = dto.FullName;

            _repository.Update(officer);
            await _repository.SaveChangesAsync();

            return true;
        }

        // Map entity to response DTO
        private static PlacementOfficerResponseDto MapToResponse(
            PlacementOfficer officer,
            ApplicationUser user,
            College college)
        {
            return new PlacementOfficerResponseDto
            {
                PlacementOfficerId = officer.PlacementOfficerId,
                UserId = officer.UserId,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                CollegeId = officer.CollegeId,
                CollegeName = college.CollegeName,
                Designation = officer.Designation,
                Status = officer.Status,
                ApprovedBy = officer.ApprovedBy,
                ApprovedAt = officer.ApprovedAt
            };
        }
    }
}