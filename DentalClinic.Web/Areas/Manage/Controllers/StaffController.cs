using DentalClinic.Application.DTOs.Common;
using DentalClinic.Application.DTOs.Staffs;
using DentalClinic.Application.Interfaces.Staffs;
using DentalClinic.Application.Interfaces.Systems;
using DentalClinic.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.Web.Areas.Manage.Controllers
{
    [ApiController]
    [Area("Manage")]
    [Route("api/[area]/[controller]")]
    [Authorize(Roles = RoleConstants.AdminManager)]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly IRoleService _roleService;

        public StaffController(IStaffService staffService, IRoleService roleService)
        {
            _staffService = staffService;
            _roleService = roleService;
        }

        [HttpGet("getpaginated")]
        public async Task<IActionResult> GetPaginatedStaff([FromQuery] PagingParams pagingParams)
        {
            var staffs = await _staffService.GetStaffPaginatedAsync(pagingParams);
            return Ok(staffs);
        }

        [HttpGet("initform")]
        public async Task<IActionResult> GetInit()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllStaff()
        {
            var staffs = await _staffService.GetAllStaffAsync();
            return Ok(staffs);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStaffById(int id)
        {
            var staff = await _staffService.GetStaffByIdAsync(id);
            if (staff == null)
                return NotFound();

            return Ok(staff);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff([FromBody] StaffCreateDto dto)
        {
            try
            {
                var staffId = await _staffService.CreateStaffAsync(dto);
                var newStaff = await _staffService.GetStaffByIdAsync(staffId);
                return CreatedAtAction(nameof(GetStaffById), new { id = staffId }, newStaff);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] StaffUpdateDto dto)
        {
            try
            {
                await _staffService.UpdateStaffAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            try
            {
                await _staffService.DeleteStaffAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
