using DentalClinic.Application.DTOs.Branches;
using DentalClinic.Application.Interfaces.Branches;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;

namespace DentalClinic.Application.Services.Branches
{
    public class BranchService : IBranchService
    {
        private readonly IBranchRepository _branchRepository;

        public BranchService(IBranchRepository branchRepository)
        {
            _branchRepository = branchRepository;
        }

        public async Task<IEnumerable<BranchDto>> GetAllBranchesAsync()
        {
            var branches = await _branchRepository.GetAllAsync();
            return branches.Select(MapToDto);
        }

        public async Task<IEnumerable<BranchDto>> GetActiveBranchesAsync()
        {
            var branches = await _branchRepository.GetActiveBranchesAsync();
            return branches.Select(MapToDto);
        }

        public async Task<BranchDto?> GetBranchByIdAsync(int branchId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            return branch != null ? MapToDto(branch) : null;
        }

        public async Task<BranchDto?> GetBranchByCodeAsync(string branchCode)
        {
            var branch = await _branchRepository.GetByBranchCodeAsync(branchCode);
            return branch != null ? MapToDto(branch) : null;
        }

        public async Task<BranchDto?> GetMainBranchAsync()
        {
            var branch = await _branchRepository.GetMainBranchAsync();
            return branch != null ? MapToDto(branch) : null;
        }

        public async Task<BranchDto> CreateBranchAsync(CreateBranchDto dto)
        {
            var branch = new Branch
            {
                BranchName = dto.BranchName,
                BranchCode = dto.BranchCode,
                Address = dto.Address,
                City = dto.City,
                District = dto.District,
                Phone = dto.Phone,
                Email = dto.Email,
                IsMainBranch = dto.IsMainBranch,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _branchRepository.AddAsync(branch);
            await _branchRepository.SaveChangesAsync();

            return MapToDto(branch);
        }

        public async Task<bool> UpdateBranchAsync(int branchId, UpdateBranchDto dto)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null) return false;

            branch.BranchName = dto.BranchName;
            branch.BranchCode = dto.BranchCode;
            branch.Address = dto.Address;
            branch.City = dto.City;
            branch.District = dto.District;
            branch.Phone = dto.Phone;
            branch.Email = dto.Email;
            branch.IsActive = dto.IsActive;
            branch.IsMainBranch = dto.IsMainBranch;
            branch.UpdatedAt = DateTime.UtcNow;

            _branchRepository.Update(branch);
            await _branchRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBranchAsync(int branchId)
        {
            var branch = await _branchRepository.GetByIdAsync(branchId);
            if (branch == null) return false;

            // Soft delete
            branch.IsActive = false;
            branch.UpdatedAt = DateTime.UtcNow;

            _branchRepository.Update(branch);
            await _branchRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserBranchDto>> GetUsersByBranchAsync(int branchId)
        {
            var mappings = await _branchRepository.GetUserBranchMappingsAsync(branchId);
            return mappings.Where(m => m.BranchId == branchId).Select(m => new UserBranchDto
            {
                UserId = m.UserId,
                UserName = m.User.Username,
                FullName = m.User.FullName,
                BranchId = m.BranchId,
                BranchName = m.Branch.BranchName,
                RoleId = m.RoleId,
                RoleName = m.Role.RoleName,
                IsActive = m.IsActive,
                AssignedAt = m.AssignedAt
            });
        }

        public async Task<IEnumerable<BranchDto>> GetBranchesByUserAsync(int userId)
        {
            var branches = await _branchRepository.GetBranchesByUserAsync(userId);
            return branches.Select(MapToDto);
        }

        public async Task<IEnumerable<UserBranchDto>> GetUserBranchMappingsAsync(int userId)
        {
            var mappings = await _branchRepository.GetUserBranchMappingsAsync(userId);
            return mappings.Select(m => new UserBranchDto
            {
                UserId = m.UserId,
                UserName = m.User.Username,
                FullName = m.User.FullName,
                BranchId = m.BranchId,
                BranchName = m.Branch.BranchName,
                RoleId = m.RoleId,
                RoleName = m.Role.RoleName,
                IsActive = m.IsActive,
                AssignedAt = m.AssignedAt
            });
        }

        public async Task<bool> AssignUserToBranchAsync(AssignUserToBranchDto dto)
        {
            return await _branchRepository.AssignUserToBranchAsync(dto.UserId, dto.BranchId, dto.RoleId);
        }

        public async Task<bool> RemoveUserFromBranchAsync(int userId, int branchId, int roleId)
        {
            return await _branchRepository.RemoveUserFromBranchAsync(userId, branchId, roleId);
        }

        private static BranchDto MapToDto(Branch branch)
        {
            return new BranchDto
            {
                BranchId = branch.BranchId,
                BranchName = branch.BranchName,
                BranchCode = branch.BranchCode,
                Address = branch.Address,
                City = branch.City,
                District = branch.District,
                Phone = branch.Phone,
                Email = branch.Email,
                IsActive = branch.IsActive,
                IsMainBranch = branch.IsMainBranch,
                CreatedAt = branch.CreatedAt
            };
        }
    }
}
