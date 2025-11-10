using DentalClinic.Application.Providers;
using DentalClinic.Domain.Common;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;

namespace DentalClinic.Infrastructure.Repositories
{
    public class UserRepository: Repository<User>, IUserRepository
    {
        private readonly ICurrentUserProvider _currentUserProvider;
        public UserRepository(DentalClinicDbContext dbContext, ICurrentUserProvider currentUserProvider) : base(dbContext)
        {
            _currentUserProvider = currentUserProvider;
        }

        public async Task<(List<User> Users, int TotalCount)> GetStaffPaginatedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            string? sortBy,
            string? sortDirection,
            List<int>? roleIds,
            List<int>? branchIds)
        {
            if(_currentUserProvider == null) return (new List<User>(), 0);

            var query = _dbSet
                .Where(u => u.UserType == UserTypeCodes.Staff.ToString())
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(u =>
                    (u.FullName != null && u.FullName.ToLower().Contains(term)) ||
                    (u.Email != null && u.Email.ToLower().Contains(term)) ||
                    (u.Phone != null && u.Phone.ToLower().Contains(term))
                );
            }

            if (branchIds != null && branchIds.Any())
            {
                query = query.Where(u =>
                    u.UserBranches != null &&
                    u.UserBranches.Any(m => branchIds.Contains(m.BranchId))
                );
            }

            if (roleIds != null && roleIds.Any())
            {
                query = query.Where(u =>
                    u.UserBranches != null &&
                    u.UserBranches.Any(m => roleIds.Contains(m.RoleId))
                );
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                bool isDescending = sortDirection?.ToLower() == "desc";

                query = (sortBy.ToLower(), isDescending) switch
                {
                    ("fullname", false) => query.OrderBy(u => u.FullName),
                    ("fullname", true) => query.OrderByDescending(u => u.FullName),
                    ("email", false) => query.OrderBy(u => u.Email),
                    ("email", true) => query.OrderByDescending(u => u.Email),
                    _ => query.OrderBy(u => u.UserId)
                };
            }
            else
            {
                query = query.OrderBy(u => u.UserId);
            }

            var totalCount = await query.CountAsync();

            var users = await query
                .Include(u => u.UserBranches!)
                    .ThenInclude(m => m.Branch)
                .Include(u => u.UserBranches!)
                    .ThenInclude(m => m.Role)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<User?> GetByIdWithAssignmentsAsync(int id)
        {
            return await _dbSet
                .Include(u => u.UserBranches!)
                    .ThenInclude(m => m.Branch)
                .Include(u => u.UserBranches!)
                    .ThenInclude(m => m.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<IEnumerable<User>> GetAllWithAssignmentsAsync()
        {
            return await _dbSet
                .Where(u => u.UserType == UserTypeCodes.Staff)
                .Include(u => u.UserBranches!)
                    .ThenInclude(m => m.Branch)
                .Include(u => u.UserBranches!)
                    .ThenInclude(m => m.Role)
                .ToListAsync();
        }


        public async Task<User?> LoginByUsernameAsync(string username)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetAllWithRolesAsync()
        {
            // Get User -> UserRoles -> UserRole
            return await _dbSet
                        .ToListAsync();
        }
        public async Task<User?> GetByIdWithRolesAsync(int id)
        {
            return await _dbSet
                        .FirstOrDefaultAsync(u => u.UserId == id);
        }
        public async Task<IEnumerable<User>> GetUsersByUserTypeAsync(string userType)
        {
            return await _dbSet
                        .Where(u => u.UserType == userType)
                        .ToListAsync();
        }
    }
}
