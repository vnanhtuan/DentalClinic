using DentalClinic.Application.DTOs;
using DentalClinic.Application.Interfaces;
using DentalClinic.Domain.Common;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;

namespace DentalClinic.Application.Services
{
    public class AuthService: IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserBranchMappingRepository _userBranchMappingRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IUserRepository userRepository,
            IUserBranchMappingRepository userBranchMappingRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository;
            _userBranchMappingRepository = userBranchMappingRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _userRepository.LoginByUsernameAsync(request.Username);
            if (user == null || !user.IsActive)
            {
                return new LoginResponseDto { Status = "Failed", Message = "Tài khoản không hợp lệ hoặc đã bị khóa." };
            }

            var isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, request.Password);
            if (!isPasswordValid)
            {
                return new LoginResponseDto { Status = "Failed", Message = "Sai tên đăng nhập hoặc mật khẩu." };
            }

            user.LastLogin = DateTime.UtcNow;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            var activeAssignments = await _userBranchMappingRepository.GetActiveAssignmentsByUserIdAsync(user.UserId);

            if (request.SelectedBranchId.HasValue)
            {
                var assignmentsForSelectedBranch = activeAssignments
                    .Where(a => a.BranchId == request.SelectedBranchId.Value)
                    .ToList();

                if (assignmentsForSelectedBranch.Any())
                {
                    // Lựa chọn hợp lệ, tạo token với TẤT CẢ các role tại chi nhánh đó
                    var token = _jwtTokenGenerator.GenerateToken(user, assignmentsForSelectedBranch);
                    return new LoginResponseDto
                    {
                        Status = "Success",
                        Token = token,
                        FullName = user.FullName,
                        Roles = assignmentsForSelectedBranch.Select(m => m.Role.RoleName).ToList()
                    };
                }
            }

            if (user.UserType == UserTypeCodes.SuperAdmin)
            {
                var token = _jwtTokenGenerator.GenerateToken(user, []);
                return new LoginResponseDto
                {
                    Status = "Success",
                    Token = token,
                    FullName = user.FullName,
                    Roles = ["Admin"]
                };
            }

            var assignmentsByBranch = activeAssignments
                .GroupBy(a => a.Branch)
                .ToList();

            if (assignmentsByBranch.Count == 0)
            {
                return new LoginResponseDto { Status = "Failed", Message = "Tài khoản chưa được phân công vào bất kỳ chi nhánh nào." };
            }
            else if (assignmentsByBranch.Count == 1)
            {
                var branchGroup = assignmentsByBranch.First();
                var assignmentsForThisBranch = branchGroup.ToList();

                var token = _jwtTokenGenerator.GenerateToken(user, assignmentsForThisBranch);
                return new LoginResponseDto
                {
                    Status = "Success",
                    Token = token,
                    FullName = user.FullName,
                    Roles = assignmentsForThisBranch.Select(m => m.Role.RoleName).ToList()
                };
            }
            else
            {
                return new LoginResponseDto
                {
                    Status = "BranchSelectionRequired",
                    Message = "Vui lòng chọn một chi nhánh để làm việc.",
                    AvailableAssignments = assignmentsByBranch.Select(group => new BranchSimpleDto
                    {
                        BranchId = group.Key.BranchId,
                        BranchName = group.Key.BranchName
                    }).ToList()
                };
            }
        }
    }
}
