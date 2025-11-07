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
        private readonly IRepository<UserRole> _roleRepository;
        private readonly IPasswordHasher _passwordHasher;

        public StaffService(IUserRepository userRepository, IRepository<UserRole> roleRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
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
                        RoleId = primaryRoleMapping?.RoleId,
                        RoleName = user.UserRoles?.FirstOrDefault()?.Role.RoleName ?? "N/A",
                        Username = user.Username,
                        CreatedAt = user.CreatedAt
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
                RoleId = primaryRoleMapping?.RoleId,
                RoleName = string.Join(", ", user.UserRoles?.Select(ur => ur.Role.RoleName) ?? Enumerable.Empty<string>())
            };
        }

        public async Task<int> CreateStaffAsync(StaffCreateDto dto)
        {
            if (await _userRepository.GetByUsernameAsync(dto.Username) != null)
                throw new Exception("Username đã tồn tại.");
            if (await _userRepository.IsEmailUniqueAsync(dto.Email) == false)
                throw new Exception("Email đã tồn tại.");

            var role = await _roleRepository.GetByIdAsync(dto.RoleId);
            if (role == null)
                throw new KeyNotFoundException($"RoleId '{dto.RoleId}' not existing.");

            // Create User Entity
            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                // Bỏ Role cũ (chỉ để lại tạm thời cho Migration)
                UserType = UserTypeCodes.Staff.ToString(),
                Username = dto.Username,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            user.UserRoles = new List<UserRoleMapping>
            {
                new UserRoleMapping { RoleId = role.RoleId }
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user.UserId;
        }

        public async Task UpdateStaffAsync(int id, StaffUpdateDto dto)
        {
            var user = await _userRepository.GetByIdWithRolesAsync(id);

            if (user == null || user.UserType != UserTypeCodes.Staff.ToString())
                throw new KeyNotFoundException("Not Found.");

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Phone = dto.Phone;

            var existingMapping = user.UserRoles?.FirstOrDefault();
            var newRole = await _roleRepository.GetByIdAsync(dto.RoleId);
            if (newRole == null)
                throw new KeyNotFoundException($"RoleId '{dto.RoleId}' not existing.");

            if (existingMapping != null)
            {
                existingMapping.RoleId = newRole.RoleId;
            }
            else
            {
                user.UserRoles = new List<UserRoleMapping>
                {
                    new UserRoleMapping { RoleId = newRole.RoleId }
                };
            }

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

    }
}
