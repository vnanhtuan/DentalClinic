using DentalClinic.Application.DTOs.Branches;
using DentalClinic.Application.Interfaces.Branches;
using DentalClinic.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.Web.Areas.Manage.Controllers
{
    [ApiController]
    [Area("Manage")]
    [Route("api/[area]/[controller]")]
    [Authorize(Roles = RoleConstants.Admin)]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        // GET: api/Branch
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetAllBranches()
        {
            var branches = await _branchService.GetAllBranchesAsync();
            return Ok(branches);
        }

        // GET: api/Branch/active
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetActiveBranches()
        {
            var branches = await _branchService.GetActiveBranchesAsync();
            return Ok(branches);
        }

        // GET: api/Branch/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BranchDto>> GetBranch(int id)
        {
            var branch = await _branchService.GetBranchByIdAsync(id);
            if (branch == null)
                return NotFound(new { message = "Branch not found" });

            return Ok(branch);
        }

        // GET: api/Branch/code/HQ
        [HttpGet("code/{code}")]
        public async Task<ActionResult<BranchDto>> GetBranchByCode(string code)
        {
            var branch = await _branchService.GetBranchByCodeAsync(code);
            if (branch == null)
                return NotFound(new { message = "Branch not found" });

            return Ok(branch);
        }

        // GET: api/Branch/main
        [HttpGet("main")]
        public async Task<ActionResult<BranchDto>> GetMainBranch()
        {
            var branch = await _branchService.GetMainBranchAsync();
            if (branch == null)
                return NotFound(new { message = "Main branch not found" });

            return Ok(branch);
        }

        // POST: api/Branch
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<BranchDto>> CreateBranch([FromBody] CreateBranchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var branch = await _branchService.CreateBranchAsync(dto);
            return CreatedAtAction(nameof(GetBranch), new { id = branch.BranchId }, branch);
        }

        // PUT: api/Branch/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBranch(int id, [FromBody] UpdateBranchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _branchService.UpdateBranchAsync(id, dto);
            if (!result)
                return NotFound(new { message = "Branch not found" });

            return NoContent();
        }

        // DELETE: api/Branch/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var result = await _branchService.DeleteBranchAsync(id);
            if (!result)
                return NotFound(new { message = "Branch not found" });

            return NoContent();
        }

        // GET: api/Branch/5/users
        [HttpGet("{id}/users")]
        public async Task<ActionResult<IEnumerable<UserBranchDto>>> GetUsersByBranch(int id)
        {
            var users = await _branchService.GetUsersByBranchAsync(id);
            return Ok(users);
        }

        // GET: api/Branch/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetBranchesByUser(int userId)
        {
            var branches = await _branchService.GetBranchesByUserAsync(userId);
            return Ok(branches);
        }

        // GET: api/Branch/user/5/mappings
        [HttpGet("user/{userId}/mappings")]
        public async Task<ActionResult<IEnumerable<UserBranchDto>>> GetUserBranchMappings(int userId)
        {
            var mappings = await _branchService.GetUserBranchMappingsAsync(userId);
            return Ok(mappings);
        }

        // POST: api/Branch/assign
        [HttpPost("assign")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignUserToBranch([FromBody] AssignUserToBranchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _branchService.AssignUserToBranchAsync(dto);
            if (!result)
                return BadRequest(new { message = "Failed to assign user to branch" });

            return Ok(new { message = "User successfully assigned to branch" });
        }

        // DELETE: api/Branch/remove
        [HttpDelete("remove")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveUserFromBranch(
            [FromQuery] int userId, 
            [FromQuery] int branchId, 
            [FromQuery] int roleId)
        {
            var result = await _branchService.RemoveUserFromBranchAsync(userId, branchId, roleId);
            if (!result)
                return NotFound(new { message = "User-branch mapping not found" });

            return Ok(new { message = "User successfully removed from branch" });
        }
    }
}
