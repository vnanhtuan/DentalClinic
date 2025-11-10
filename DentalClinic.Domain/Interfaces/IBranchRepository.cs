using DentalClinic.Domain.Entities;

namespace DentalClinic.Domain.Interfaces
{
    public interface IBranchRepository : IRepository<Branch>
    {
        Task<Branch?> GetByBranchCodeAsync(string branchCode);
        Task<IEnumerable<Branch>> GetActiveBranchesAsync();
        Task<Branch?> GetMainBranchAsync();
        Task<IEnumerable<User>> GetUsersByBranchAsync(int branchId);
        Task<IEnumerable<Branch>> GetBranchesByUserAsync(int userId);
        Task<bool> AssignUserToBranchAsync(int userId, int branchId, int roleId);
        Task<bool> RemoveUserFromBranchAsync(int userId, int branchId, int roleId);
        Task<IEnumerable<UserBranchMapping>> GetUserBranchMappingsAsync(int userId);
        Task<List<Branch>> GetBranchesByUserIdAsync(int userId);
    }
}
