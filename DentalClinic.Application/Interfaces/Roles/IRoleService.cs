using DentalClinic.Application.DTOs.Roles;

namespace DentalClinic.Application.Interfaces.Roles
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<int> CreateRoleAsync(RoleCreateUpdateDto dto);
        Task UpdateRoleAsync(int id, RoleCreateUpdateDto dto);
        Task DeleteRoleAsync(int id);
    }
}
