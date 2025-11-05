using DentalClinic.Application.DTOs;
using DentalClinic.Application.Interfaces;
using DentalClinic.Domain.Common;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;

namespace DentalClinic.Application.Services
{
    public class UserManagementService: IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserManagementService(IUserRepository userRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }
        public async Task<int> RegisterPatientAsync(RegisterPatientDto request)
        {
            if (await _userRepository.GetByUsernameAsync(request.Username) != null)
                throw new Exception("Username đã tồn tại.");
            if (await _userRepository.IsEmailUniqueAsync(request.Email) == false)
                throw new Exception("Email đã tồn tại.");

            var hashedPassword = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                FullName = request.FullName,
                Username = request.Username,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = hashedPassword,
                UserType = UserTypeCodes.Patient,
                IsActive = true,
                PatientDetail = new PatientDetail
                {
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth,
                    Address = request.Address
                }
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user.UserId;
        }

        public async Task<int> RegisterStaffAsync(RegisterStaffDto request)
        {
            if (await _userRepository.GetByUsernameAsync(request.Username) != null)
                throw new Exception("Username đã tồn tại.");
            if (await _userRepository.IsEmailUniqueAsync(request.Email) == false)
                throw new Exception("Email đã tồn tại.");

            var hashedPassword = _passwordHasher.HashPassword(request.Password);

            var user = new User
            {
                FullName = request.FullName,
                Username = request.Username,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = hashedPassword,
                UserType = UserTypeCodes.Staff,
                IsActive = true,
                StaffDetail = new StaffDetail
                {
                    RoleTitle = request.RoleTitle,
                    Department = request.Department
                }
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user.UserId;
        }
    }
}
