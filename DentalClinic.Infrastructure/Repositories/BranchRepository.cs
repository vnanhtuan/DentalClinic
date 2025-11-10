using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Repositories
{
    public class BranchRepository : Repository<Branch>, IBranchRepository
    {
        public BranchRepository(DentalClinicDbContext context) : base(context)
        {
        }

        public async Task<Branch?> GetByBranchCodeAsync(string branchCode)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(b => b.BranchCode == branchCode);
        }

        public async Task<IEnumerable<Branch>> GetActiveBranchesAsync()
        {
            return await _dbSet.AsNoTracking()
                .Where(b => b.IsActive)
                .OrderBy(b => b.BranchName)
                .ToListAsync();
        }

        public async Task<Branch?> GetMainBranchAsync()
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(b => b.IsMainBranch && b.IsActive);
        }

        public async Task<IEnumerable<User>> GetUsersByBranchAsync(int branchId)
        {
            return await _context.UserBranchMappings
                .Where(ub => ub.BranchId == branchId && ub.IsActive)
                .Include(ub => ub.User)
                .Include(ub => ub.Role)
                .Select(ub => ub.User)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<Branch>> GetBranchesByUserAsync(int userId)
        {
            return await _context.UserBranchMappings
                .Where(ub => ub.UserId == userId && ub.IsActive)
                .Include(ub => ub.Branch)
                .Select(ub => ub.Branch)
                .Distinct()
                .ToListAsync();
        }

        public async Task<bool> AssignUserToBranchAsync(int userId, int branchId, int roleId)
        {
            var existingMapping = await _context.UserBranchMappings
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BranchId == branchId && ub.RoleId == roleId);

            if (existingMapping != null)
            {
                // Reactivate if exists
                existingMapping.IsActive = true;
                existingMapping.AssignedAt = DateTime.UtcNow;
                existingMapping.RemovedAt = null;
            }
            else
            {
                var mapping = new UserBranchMapping
                {
                    UserId = userId,
                    BranchId = branchId,
                    RoleId = roleId,
                    IsActive = true,
                    AssignedAt = DateTime.UtcNow
                };
                await _context.UserBranchMappings.AddAsync(mapping);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveUserFromBranchAsync(int userId, int branchId, int roleId)
        {
            var mapping = await _context.UserBranchMappings
                .FirstOrDefaultAsync(ub => ub.UserId == userId && ub.BranchId == branchId && ub.RoleId == roleId);

            if (mapping != null)
            {
                mapping.IsActive = false;
                mapping.RemovedAt = DateTime.UtcNow;
                return await _context.SaveChangesAsync() > 0;
            }

            return false;
        }

        public async Task<IEnumerable<UserBranchMapping>> GetUserBranchMappingsAsync(int userId)
        {
            return await _context.UserBranchMappings
                .Where(ub => ub.UserId == userId && ub.IsActive)
                .Include(ub => ub.Branch)
                .Include(ub => ub.Role)
                .ToListAsync();
        }

        public async Task<List<Branch>> GetBranchesByUserIdAsync(int userId)
        {
            return await _context.UserBranchMappings
                .Where(ubm => ubm.UserId == userId && ubm.IsActive)
                .Include(ubm => ubm.Branch)
                .Select(ubm => ubm.Branch)
                .Distinct()
                .ToListAsync();
        }
    }
}
