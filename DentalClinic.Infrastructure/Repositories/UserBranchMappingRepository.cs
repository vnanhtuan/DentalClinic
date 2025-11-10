using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Repositories
{
    public class UserBranchMappingRepository: Repository<UserBranchMapping>, IUserBranchMappingRepository
    {
        public UserBranchMappingRepository(DentalClinicDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<IEnumerable<UserBranchMapping>> GetActiveAssignmentsByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(m => m.Branch)
                .Include(m => m.Role)
                .Where(m => m.UserId == userId && m.IsActive)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserBranchMapping?> GetAssignmentAsync(int userId, int branchId, int roleId)
        {
            return await _dbSet
                .Include(m => m.Branch)
                .Include(m => m.Role)
                .AsNoTracking()
                .FirstOrDefaultAsync(m =>
                    m.UserId == userId &&
                    m.BranchId == branchId &&
                    m.RoleId == roleId &&
                    m.IsActive);
        }
    }
}
