using DentalClinic.Application.DTOs.Branches;

namespace DentalClinic.Application.Interfaces.Branches
{
    public interface IBranchService
    {
        Task<IEnumerable<BranchDto>> GetAllBranchesAsync();
        Task<IEnumerable<BranchDto>> GetActiveBranchesAsync();
        Task<BranchDto?> GetBranchByIdAsync(int branchId);
        Task<BranchDto?> GetBranchByCodeAsync(string branchCode);
        Task<BranchDto?> GetMainBranchAsync();
        Task<BranchDto> CreateBranchAsync(CreateBranchDto dto);
        Task<bool> UpdateBranchAsync(int branchId, UpdateBranchDto dto);
        Task<bool> DeleteBranchAsync(int branchId);
        
        // User-Branch Management
        Task<IEnumerable<UserBranchDto>> GetUsersByBranchAsync(int branchId);
        Task<IEnumerable<BranchDto>> GetBranchesByUserAsync(int userId);
        Task<IEnumerable<UserBranchDto>> GetUserBranchMappingsAsync(int userId);
        Task<bool> AssignUserToBranchAsync(AssignUserToBranchDto dto);
        Task<bool> RemoveUserFromBranchAsync(int userId, int branchId, int roleId);
    }
}
