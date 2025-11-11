using DentalClinic.Application.DTOs.Common;
using DentalClinic.Application.Modules.Roles.DTOs;

namespace DentalClinic.Application.Interfaces.Roles
{
    public interface IRoleService
    {
        Task<PagingResponse<RoleDto>> GetRolePaginatedAsync(BasePagingParams pagingParams);
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto?> GetRoleByIdAsync(int id);
        Task<int> CreateRoleAsync(RoleCreateUpdateDto dto);
        Task UpdateRoleAsync(int id, RoleCreateUpdateDto dto);
        Task DeleteRoleAsync(int id);
    }
}
