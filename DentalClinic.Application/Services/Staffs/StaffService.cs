using DentalClinic.Application.DTOs.Branches;
using DentalClinic.Application.DTOs.Common;
using DentalClinic.Application.DTOs.Roles;
using DentalClinic.Application.DTOs.Staffs;
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
        private readonly IUserBranchMappingRepository _mappingRepository;
        private readonly IRepository<Branch> _branchRepository;
        private readonly IRepository<UserRole> _roleRepository;
        private readonly IPasswordHasher _passwordHasher;

        public StaffService(IUserRepository userRepository, IUserBranchMappingRepository mappingRepository,
            IRepository<Branch> branchRepository, IRepository<UserRole> roleRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _branchRepository = branchRepository;
            _mappingRepository = mappingRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<PagingResponse<StaffDto>> GetStaffPaginatedAsync(PagingParams pagingParams)
        {
            var (users, totalItems) = await _userRepository.GetStaffPaginatedAsync(
                pagingParams.PageNumber,
                pagingParams.PageSize,
                pagingParams.SearchTerm,
                pagingParams.SortBy,
                pagingParams.SortDirection,
                pagingParams.RoleIds,
                pagingParams.BranchIds
            );

            var staffDtos = users.Select(MapUserToStaffDto).ToList();

            return new PagingResponse<StaffDto>(
                staffDtos,
                totalItems,
                pagingParams.PageNumber,
                pagingParams.PageSize
            );
        }

        public async Task<IEnumerable<StaffDto>> GetAllStaffAsync()
        {
            var users = await _userRepository.GetAllWithAssignmentsAsync();

            return users
                .Where(u => u.UserType == UserTypeCodes.Staff.ToString())
                .Select(MapUserToStaffDto)
                .ToList();
        }

        public async Task<StaffDto?> GetStaffByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdWithAssignmentsAsync(id);

            if (user == null || user.UserType != UserTypeCodes.Staff.ToString())
            {
                return null;
            }

            return MapUserToStaffDto(user);
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
                IsActive = true
            };

            user.UserBranches = new List<UserBranchMapping>();
            var distinctBranchIds = dto.BranchIds.Distinct();
            var distinctRoleIds = dto.RoleIds.Distinct();

            foreach (var branchId in distinctBranchIds)
            {
                foreach (var roleId in distinctRoleIds)
                {
                    user.UserBranches.Add(new UserBranchMapping
                    {
                        BranchId = branchId,
                        RoleId = roleId,
                        IsActive = true,
                        AssignedAt = DateTime.UtcNow
                    });
                }
            }

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user.UserId;
        }

        public async Task UpdateStaffAsync(int id, StaffUpdateDto dto)
        {
            var user = await _userRepository.GetByIdWithAssignmentsAsync(id);

            if (user == null || user.UserType != UserTypeCodes.Staff.ToString())
                throw new KeyNotFoundException("Not Found.");

            //if (!await _userRepository.IsEmailUniqueAsync(dto.Email, id))
            //    throw new DbUpdateException("Email đã được sử dụng bởi tài khoản khác.");

            await ValidateRoleIds(dto.RoleIds);

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Phone = dto.Phone ?? "";
            user.UserBranches ??= [];

            SyncUserAssignments(user, dto.BranchIds, dto.RoleIds);

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteStaffAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null || user.UserType != UserTypeCodes.Staff.ToString())
                throw new KeyNotFoundException("Not Found");

            if (user.UserType == UserTypeCodes.SuperAdmin)
                throw new InvalidOperationException("Không thể xóa SuperAdmin.");

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

        private StaffDto MapUserToStaffDto(User user)
        {
            var assignments = user.UserBranches ?? [];

            return new StaffDto
            {
                Id = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Username = user.Username,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,

                Branches = assignments
                    .Select(m => m.Branch)
                    .DistinctBy(b => b.BranchId)
                    .Select(b => new BranchDto
                    {
                        BranchId = b.BranchId,
                        BranchName = b.BranchName
                    }).ToList(),

                Roles = assignments
                    .Select(m => m.Role)
                    .DistinctBy(r => r.RoleId)
                    .Select(r => new RoleDto
                    {
                        RoleId = r.RoleId,
                        Name = r.RoleName,
                        Color = r.Color
                    }).ToList()
            };
        }

        private void SyncUserAssignments(User user, List<int> newBranchIds, List<int> newRoleIds)
        {
            var distinctBranchIds = newBranchIds.Distinct().ToList();
            var distinctRoleIds = newRoleIds.Distinct().ToList();

            var desiredMappings = new HashSet<(int BranchId, int RoleId)>();
            foreach (var branchId in distinctBranchIds)
            {
                foreach (var roleId in distinctRoleIds)
                {
                    desiredMappings.Add((branchId, roleId));
                }
            }

            var existingMappings = user.UserBranches!
                        .Select(m => (m.BranchId, m.RoleId))
                        .ToHashSet();

            var mappingsToRemove = user.UserBranches!
                .Where(m => !desiredMappings.Contains((m.BranchId, m.RoleId)))
                .ToList();

            foreach (var mapping in mappingsToRemove)
            {
                _mappingRepository.Delete(mapping);
            }

            var mappingsToAdd = desiredMappings
                .Where(dm => !existingMappings.Contains(dm))
                .ToList();

            foreach (var (branchId, roleId) in mappingsToAdd)
            {
                user.UserBranches!.Add(new UserBranchMapping
                {
                    UserId = user.UserId,
                    BranchId = branchId,
                    RoleId = roleId,
                    IsActive = true,
                    AssignedAt = DateTime.UtcNow
                });
            }
        }
    }
}
