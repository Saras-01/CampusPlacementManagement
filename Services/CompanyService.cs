using CampusPlacementManagement.DTOs.Company;
using CampusPlacementManagement.Enums;
using CampusPlacementManagement.Models;
using CampusPlacementManagement.Repositories;

namespace CampusPlacementManagement.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _repository;

        public CompanyService(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public async Task<CompanyResponseDto> RegisterAsync(
            CompanyRegisterDto dto)
        {
            var existingCompany =
                await _repository.GetByEmailAsync(dto.CompanyEmail);

            if (existingCompany != null)
            {
                throw new InvalidOperationException(
                    "A company with this email already exists.");
            }

            var company = new Company
            {
                CompanyName = dto.CompanyName,
                CompanyEmail = dto.CompanyEmail,
                Website = dto.Website,
                Industry = dto.Industry,
                Address = dto.Address,
                Description = dto.Description,
                Status = AccountStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(company);
            await _repository.SaveChangesAsync();

            return MapToResponse(company);
        }

        public async Task<List<CompanyResponseDto>> GetAllAsync()
        {
            var companies = await _repository.GetAllAsync();

            return companies.Select(MapToResponse).ToList();
        }

        public async Task<CompanyResponseDto?> GetByIdAsync(int id)
        {
            var company = await _repository.GetByIdAsync(id);

            return company == null ? null : MapToResponse(company);
        }

        public async Task<List<CompanyResponseDto>> GetPendingAsync()
        {
            var companies = await _repository.GetPendingAsync();

            return companies.Select(MapToResponse).ToList();
        }

        public async Task<bool> ApproveAsync(int id)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company == null)
                return false;

            if (company.Status != AccountStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending companies can be approved.");
            }

            company.Status = AccountStatus.Approved;

            _repository.Update(company);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectAsync(int id)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company == null)
                return false;

            if (company.Status != AccountStatus.Pending)
            {
                throw new InvalidOperationException(
                    "Only pending companies can be rejected.");
            }

            company.Status = AccountStatus.Rejected;

            _repository.Update(company);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SuspendAsync(int id)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company == null)
                return false;

            if (company.Status != AccountStatus.Approved)
            {
                throw new InvalidOperationException(
                    "Only approved companies can be suspended.");
            }

            company.Status = AccountStatus.Suspended;

            _repository.Update(company);
            await _repository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(
            int id,
            CompanyUpdateDto dto)
        {
            var company = await _repository.GetByIdAsync(id);

            if (company == null)
                return false;

            var existingCompany =
                await _repository.GetByEmailAsync(dto.CompanyEmail);

            if (existingCompany != null &&
                existingCompany.CompanyId != id)
            {
                throw new InvalidOperationException(
                    "Another company is already using this email.");
            }

            company.CompanyName = dto.CompanyName;
            company.CompanyEmail = dto.CompanyEmail;
            company.Website = dto.Website;
            company.Industry = dto.Industry;
            company.Address = dto.Address;
            company.Description = dto.Description;

            _repository.Update(company);
            await _repository.SaveChangesAsync();

            return true;
        }

        private static CompanyResponseDto MapToResponse(Company company)
        {
            return new CompanyResponseDto
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                CompanyEmail = company.CompanyEmail,
                Website = company.Website,
                Industry = company.Industry,
                Address = company.Address,
                Description = company.Description,
                Status = company.Status,
                CreatedAt = company.CreatedAt
            };
        }
    }
}