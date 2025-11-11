using DentalClinic.Application.DTOs.Common;
using DentalClinic.Application.Interfaces.Roles;
using DentalClinic.Application.Modules.Roles.DTOs;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DentalClinic.Application.Modules.Roles
{
    public class RoleService: IRoleService
    {
        private readonly IRepository<UserRole> _roleRepository;
        private readonly IRepository<UserBranchMapping> _userBranchMappingrepository;
        private readonly IMemoryCache _cache;

        private const string AllRolesCacheKey = "AllRolesCacheKey";

        public RoleService(IRepository<UserRole> roleRepository, IRepository<UserBranchMapping> userBranchMappingrepository,
            IMemoryCache cache)
        {
            _roleRepository = roleRepository;
            _userBranchMappingrepository = userBranchMappingrepository;
            _cache = cache;
        }
        public async Task<PagingResponse<RoleDto>> GetRolePaginatedAsync(BasePagingParams pagingParams)
        {
            var roles = await GetAllRolesAsync();

            return new PagingResponse<RoleDto>(
                [..roles],
                roles.Count(),
                pagingParams.PageNumber,
                pagingParams.PageSize
            );
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            if (_cache.TryGetValue(AllRolesCacheKey, out List<RoleDto>? cachedRoles) && cachedRoles != null)
            {
                return cachedRoles;
            }
            var roles = await _roleRepository.GetAllAsync();
            var roleDtos = roles.Select(MapRoleToRoleDto).ToList();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));

            _cache.Set(AllRolesCacheKey, roleDtos, cacheEntryOptions);

            return roleDtos;
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

            InvalidateRolesCache();

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

            InvalidateRolesCache();
        }
        public async Task DeleteRoleAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                throw new KeyNotFoundException("Not Found");

            var isRoleInUse = await _userBranchMappingrepository.FindAsync(m => m.RoleId == id);
            if (isRoleInUse != null)
            {
                throw new InvalidOperationException($"Can't delete '{role.RoleName}' cause existing user have this role");
            }

            _roleRepository.Delete(role);
            await _roleRepository.SaveChangesAsync();

            InvalidateRolesCache();
        }
        private RoleDto MapRoleToRoleDto(UserRole user)
        {
            return new RoleDto
            {
                Name = user.RoleName,
                RoleId = user.RoleId,
                Color = user.Color,
                Description = user.Description
            };
        }

        private void InvalidateRolesCache()
        {
            _cache.Remove(AllRolesCacheKey);
        }
    }
}
