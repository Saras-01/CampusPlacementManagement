
using CampusPlacementManagement.DTOs.College;
using CampusPlacementManagement.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CampusPlacementManagement.Controllers
{
    [Route("api/colleges")]
    [ApiController]
    public class CollegesController : ControllerBase
    {
        private readonly ICollegeService _collegeService;

        public CollegesController(ICollegeService collegeService)
        {
            _collegeService = collegeService;
        }

        // 1. Register College
        // POST: api/colleges/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCollege(
            [FromBody] CollegeRegisterDto dto)
        {
            try
            {
                var result = await _collegeService.RegisterCollegeAsync(dto);

                return CreatedAtAction(
                    nameof(GetCollegeById),
                    new { id = result.CollegeId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        // 2. Get All Colleges
        // GET: api/colleges
        [HttpGet]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetAllColleges()
        {
            var colleges = await _collegeService.GetAllCollegesAsync();

            return Ok(colleges);
        }

        // 3. Get College By ID
        // GET: api/colleges/{id}
        [HttpGet("{id:int}")]
        [Authorize(Roles = "SuperAdmin,PlacementOfficer")]
        public async Task<IActionResult> GetCollegeById(int id)
        {
            var college = await _collegeService.GetCollegeByIdAsync(id);

            if (college == null)
                return NotFound(new { message = "College not found." });

            return Ok(college);
        }

        // 4. Get Pending Colleges
        // GET: api/colleges/pending
        [HttpGet("pending")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> GetPendingColleges()
        {
            var colleges = await _collegeService.GetPendingCollegesAsync();

            return Ok(colleges);
        }

        // 5. Approve College
        // PUT: api/colleges/{id}/approve
        [HttpPut("{id:int}/approve")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ApproveCollege(int id)
        {
            var result = await _collegeService.ApproveCollegeAsync(id);

            if (!result)
                return NotFound(new { message = "College not found." });

            return Ok(new { message = "College approved successfully." });
        }

        // 6. Reject College
        // PUT: api/colleges/{id}/reject
        [HttpPut("{id:int}/reject")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> RejectCollege(int id)
        {
            var result = await _collegeService.RejectCollegeAsync(id);

            if (!result)
                return NotFound(new { message = "College not found." });

            return Ok(new { message = "College rejected successfully." });
        }

        // 7. Suspend College
        // PUT: api/colleges/{id}/suspend
        [HttpPut("{id:int}/suspend")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> SuspendCollege(int id)
        {
            var result = await _collegeService.SuspendCollegeAsync(id);

            if (!result)
                return NotFound(new { message = "College not found." });

            return Ok(new { message = "College suspended successfully." });
        }

        // 8. Update College
        // PUT: api/colleges/{id}
        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> UpdateCollege(
            int id,
            [FromBody] CollegeUpdateDto dto)
        {
            try
            {
                var result = await _collegeService.UpdateCollegeAsync(id, dto);

                if (!result)
                    return NotFound(new { message = "College not found." });

                return Ok(new { message = "College updated successfully." });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }
    }
}
