using DentalClinic.Domain.Entities;

namespace DentalClinic.Domain.Interfaces
{
    public interface IUserBranchMappingRepository : IRepository<UserBranchMapping>
    {
        Task<IEnumerable<UserBranchMapping>> GetActiveAssignmentsByUserIdAsync(int userId);

        Task<UserBranchMapping?> GetAssignmentAsync(int userId, int branchId, int roleId);
    }
}
