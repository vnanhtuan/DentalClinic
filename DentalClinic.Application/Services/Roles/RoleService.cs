using DentalClinic.Application.DTOs.Roles;
using DentalClinic.Application.Interfaces.Systems;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;

namespace DentalClinic.Application.Services.Roles
{
    public class RoleService: IRoleService
    {
        private readonly IRepository<UserRole> _roleRepository;
        private readonly IRepository<UserRoleMapping> _userRoleMappingrepository;

        public RoleService(IRepository<UserRole> roleRepository, IRepository<UserRoleMapping> userRoleMappingrepository)
        {
            _roleRepository = roleRepository;
            _userRoleMappingrepository = userRoleMappingrepository;
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles
                .Select(role => new RoleDto
                {
                    RoleId = role.RoleId,
                    Name = role.RoleName,
                    Color = role.Color,
                    Description = role.Description,
                });
        }

        public async Task<RoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null) return null;
            return new RoleDto
            {
                RoleId = role.RoleId,
                Name = role.RoleName,
                Color = role.Color,
                Description = role.Description,
            };
        }
        public async Task<int> CreateRoleAsync(RoleCreateUpdateDto dto)
        {
            var existing = await _roleRepository.FindAsync(r => r.RoleName == dto.Name);
            if (existing != null && existing.Any())
                throw new Exception("RoleName is existing.");

            var role = new UserRole
            {
                RoleName = dto.Name,
                Color = dto.Color,
                Description = dto.Description,
            };

            await _roleRepository.AddAsync(role);
            await _roleRepository.SaveChangesAsync();

            return role.RoleId;
        }
        public async Task UpdateRoleAsync(int id, RoleCreateUpdateDto dto)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                throw new KeyNotFoundException("Not Found");

            var existing = await _roleRepository.FindAsync(r => r.RoleName == dto.Name && r.RoleId != id);
            if (existing != null && existing.Any())
                throw new Exception("RoleName is existing.");

            role.RoleName = dto.Name;
            role.Color = dto.Color;
            role.Description = dto.Description;

            _roleRepository.Update(role);
            await _roleRepository.SaveChangesAsync();
        }
        public async Task DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                throw new KeyNotFoundException("Not Found");

            var isRoleInUse = await _userRoleMappingrepository.FindAsync(m => m.RoleId == id);
            if (isRoleInUse != null)
            {
                throw new InvalidOperationException($"Can't delete '{role.RoleName}' cause existing user have this role");
            }

            _roleRepository.Delete(role);
            await _roleRepository.SaveChangesAsync();
        }
    }
}
