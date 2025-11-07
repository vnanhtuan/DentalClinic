using DentalClinic.Application.DTOs.Systems;
using DentalClinic.Application.Interfaces.Systems;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.Web.Areas.Manage.Controllers
{
    [ApiController]
    [Area("Manage")]
    [Route("api/[area]/[controller]")]
    public class RoleController: ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            return Ok(await _roleService.GetAllRolesAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRoleById(int id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            return role == null ? NotFound() : Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateUpdateDto dto)
        {
            try
            {
                var roleId = await _roleService.CreateRoleAsync(dto);
                var newRole = await _roleService.GetRoleByIdAsync(roleId);
                return CreatedAtAction(nameof(GetRoleById), new { id = roleId }, newRole);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] RoleCreateUpdateDto dto)
        {
            try
            {
                await _roleService.UpdateRoleAsync(id, dto);
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
        public async Task<IActionResult> DeleteRole(int id)
        {
            try
            {
                await _roleService.DeleteRoleAsync(id);
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
    }
}
