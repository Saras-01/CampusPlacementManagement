using CampusPlacementManagement.Constants;
using CampusPlacementManagement.DTOs.Company;
using CampusPlacementManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _service;

        public CompaniesController(ICompanyService service)
        {
            _service = service;
        }

        // Public company registration
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            CompanyRegisterDto dto)
        {
            try
            {
                var result = await _service.RegisterAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.CompanyId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Get all companies
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // Get company by ID
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Company not found." });

            return Ok(result);
        }

        // Get pending companies
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _service.GetPendingAsync();
            return Ok(result);
        }

        // Approve company
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var result = await _service.ApproveAsync(id);

                if (!result)
                    return NotFound(new { message = "Company not found." });

                return Ok(new { message = "Company approved successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Reject company
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var result = await _service.RejectAsync(id);

                if (!result)
                    return NotFound(new { message = "Company not found." });

                return Ok(new { message = "Company rejected successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Suspend company
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id}/suspend")]
        public async Task<IActionResult> Suspend(int id)
        {
            try
            {
                var result = await _service.SuspendAsync(id);

                if (!result)
                    return NotFound(new { message = "Company not found." });

                return Ok(new { message = "Company suspended successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Update company
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            CompanyUpdateDto dto)
        {
            try
            {
                var result = await _service.UpdateAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "Company not found." });

                return Ok(new { message = "Company updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}