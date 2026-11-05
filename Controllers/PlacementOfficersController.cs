using System.Security.Claims;

using CampusPlacementManagement.DTOs.PlacementOfficer;
using CampusPlacementManagement.Interfaces;
using CampusPlacementManagement.Constants;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampusPlacementManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacementOfficersController : ControllerBase
    {
        private readonly IPlacementOfficerService _service;

        public PlacementOfficersController(
            IPlacementOfficerService service)
        {
            _service = service;
        }

        // 1. Register Placement Officer
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(
            PlacementOfficerRegisterDto dto)
        {
            try
            {
                var result = await _service.RegisterAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.PlacementOfficerId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. Get all Placement Officers
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // 3. Get Placement Officer by ID
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Placement Officer not found." });

            return Ok(result);
        }

        // 4. Get pending Placement Officers
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPending()
        {
            var result = await _service.GetPendingAsync();
            return Ok(result);
        }

        // 5. Get Placement Officers by College
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpGet("college/{collegeId}")]
        public async Task<IActionResult> GetByCollegeId(int collegeId)
        {
            var result = await _service.GetByCollegeIdAsync(collegeId);
            return Ok(result);
        }

        // 6. Approve Placement Officer
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                var approvedBy = User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(approvedBy))
                    return Unauthorized();

                var result = await _service.ApproveAsync(id, approvedBy);

                if (!result)
                    return NotFound(new { message = "Placement Officer not found." });

                return Ok(new
                {
                    message = "Placement Officer approved successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 7. Reject Placement Officer
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                var result = await _service.RejectAsync(id);

                if (!result)
                    return NotFound(new { message = "Placement Officer not found." });

                return Ok(new
                {
                    message = "Placement Officer rejected successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 8. Update Placement Officer
        [Authorize(Roles = UserRoles.SuperAdmin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id, PlacementOfficerUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);

            if (!result)
                return NotFound(new { message = "Placement Officer not found." });

            return Ok(new
            {
                message = "Placement Officer updated successfully."
            });
        }
    }
}