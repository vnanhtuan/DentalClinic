using DentalClinic.Application.DTOs.Common;
using DentalClinic.Application.DTOs.Staffs;
using DentalClinic.Application.DTOs.Roles;
using DentalClinic.Application.Interfaces;
using DentalClinic.Application.Interfaces.Staffs;
using DentalClinic.Domain.Common;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;

namespace DentalClinic.Application.Services.Staffs
{
    public class StaffService : IStaffService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRepository<UserRole> _roleRepository;
        private readonly IPasswordHasher _passwordHasher;

        public StaffService(IUserRepository userRepository, IRepository<UserRole> roleRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<PagingResponse<StaffDto>> GetStaffPaginatedAsync(PagingParams pagingParams)
        {
            var (users, totalItems) = await _userRepository.GetStaffPaginatedAsync(
                pagingParams.PageNumber,
                pagingParams.PageSize,
                pagingParams.SearchTerm,
                pagingParams.SortBy,
                pagingParams.SortDirection
            );

            var staffDtos = users.Select(user =>
            {
                return new StaffDto
                {
                    Id = user.UserId,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Username = user.Username,
                    CreatedAt = user.CreatedAt,
                    Roles = user.UserRoles?.Select(ur => new RoleDto
                    {
                        RoleId = ur.RoleId,
                        Name = ur.Role.RoleName,
                        Color = ur.Role.Color
                    }).ToList() ?? []
                };
            }).ToList();

            return new PagingResponse<StaffDto>(
                staffDtos,
                totalItems,
                pagingParams.PageNumber,
                pagingParams.PageSize
            );
        }

        public async Task<IEnumerable<StaffDto>> GetAllStaffAsync()
        {
            var users = await _userRepository.GetAllWithRolesAsync();

            return users
                .Where(u => u.UserType == UserTypeCodes.Staff.ToString())
                .Select(user => {
                    var primaryRoleMapping = user.UserRoles?.FirstOrDefault();
                    return new StaffDto
                    {
                        Id = user.UserId,
                        FullName = user.FullName,
                        Email = user.Email,
                        Phone = user.Phone,
                        RoleName = primaryRoleMapping?.Role.RoleName ?? "N/A",
                        RoleColor = primaryRoleMapping?.Role.Color ?? "",
                        Username = user.Username,
                        CreatedAt = user.CreatedAt,
                        Roles = user.UserRoles?.Select(ur => new RoleDto
                        {
                            RoleId = ur.RoleId,
                            Name = ur.Role.RoleName,
                            Color = ur.Role.Color
                        }).ToList() ?? []
                    };
                });
        }

        public async Task<StaffDto?> GetStaffByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(id);

            if (user == null || user.UserType != UserTypeCodes.Staff.ToString())
            {
                return null;
            }
            var primaryRoleMapping = user.UserRoles?.FirstOrDefault();

            return new StaffDto
            {
                Id = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Username = user.Username,
                CreatedAt = user.CreatedAt,
                Roles = user.UserRoles?.Select(ur => new RoleDto
                {
                    RoleId = ur.RoleId,
                    Name = ur.Role.RoleName,
                    Color = ur.Role.Color
                }).ToList() ?? new List<RoleDto>()
            };
        }

        public async Task<int> CreateStaffAsync(StaffCreateDto dto)
        {
            if (await _userRepository.GetByUsernameAsync(dto.Username) != null)
                throw new Exception("Username đã tồn tại.");
            if (await _userRepository.IsEmailUniqueAsync(dto.Email) == false)
                throw new Exception("Email đã tồn tại.");

            await ValidateRoleIds(dto.RoleIds);

            // Create User Entity
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone ?? "",
                UserType = UserTypeCodes.Staff.ToString(),
                Username = dto.Username,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow,
                UserRoles = []
            };

            foreach (var roleId in dto.RoleIds.Distinct())
            {
                user.UserRoles.Add(new UserRoleMapping { RoleId = roleId });
            }

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user.UserId;
        }

        public async Task UpdateStaffAsync(int id, StaffUpdateDto dto)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(id);

            if (user == null || user.UserType != UserTypeCodes.Staff.ToString())
                throw new KeyNotFoundException("Not Found.");

            await ValidateRoleIds(dto.RoleIds);

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Phone = dto.Phone ?? "";

            SyncUserRoles(user, dto.RoleIds);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteStaffAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null || user.UserType != UserTypeCodes.Staff.ToString())
                throw new KeyNotFoundException("Not Found");

            _userRepository.Delete(user);
            await _userRepository.SaveChangesAsync();
        }

        private async Task ValidateRoleIds(List<int> roleIds)
        {
            if (roleIds == null || !roleIds.Any())
            {
                return;
            }

            var distinctRoleIds = roleIds.Distinct().ToList();
            var roles = await _roleRepository.GetAllAsync();
            var foundRoles = roles.Where(r => distinctRoleIds.Contains(r.RoleId)).ToList();

            if (foundRoles.Count != distinctRoleIds.Count)
            {
                var missingIds = distinctRoleIds.Except(foundRoles.Select(r => r.RoleId));
                throw new KeyNotFoundException($"Not found RoleId: {string.Join(", ", missingIds)}");
            }
        }

        private void SyncUserRoles(User user, List<int> newRoleIds)
        {
            var distinctNewRoleIds = newRoleIds.Distinct().ToList();
            
            user.UserRoles ??= [];

            // 1. Get all RoleIds existing
            var existingRoleIds = user.UserRoles.Select(m => m.RoleId).ToList() ?? [];

            // 2. Find RoleId add new
            var rolesToAddIds = distinctNewRoleIds.Except(existingRoleIds).ToList();

            // 3. Find RoleMapping to remove
            var rolesToRemoveIds = existingRoleIds.Except(distinctNewRoleIds).ToList();
            var mappingsToRemove = user.UserRoles
                                      .Where(m => rolesToRemoveIds.Contains(m.RoleId))
                                      .ToList() ?? [];

            foreach (var mapping in mappingsToRemove)
            {
                user.UserRoles.Remove(mapping);
            }

            foreach (var roleId in rolesToAddIds)
            {
                user.UserRoles.Add(new UserRoleMapping { RoleId = roleId, UserId = user.UserId });
            }
        }
    }
}
