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
        public UserRepository(DentalClinicDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<(List<User> Users, int TotalCount)> GetStaffPaginatedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            string? sortBy,
            string? sortDirection)
        {
            var query = _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.UserType == UserTypeCodes.Staff.ToString())
                .AsNoTracking();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(u =>
                    u.FullName.Contains(term) ||
                    u.Email.Contains(term) ||
                    (u.Phone != null && u.Phone.Contains(term)) ||
                    (u.UserRoles != null && u.UserRoles.Any(ur => ur.Role.RoleName.Contains(term)))
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
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }
        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public async Task<User?> LoginByUsernameAsync(string username)
        {
            return await _dbSet.AsNoTracking()
                .Include(m => m.UserRoles)
                    .ThenInclude(ur => ur.Role)
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
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                        .ToListAsync();
        }
        public async Task<User?> GetByIdWithRolesAsync(int id)
        {
            return await _dbSet
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                        .FirstOrDefaultAsync(u => u.UserId == id);
        }
        public async Task<IEnumerable<User>> GetUsersByUserTypeAsync(string userType)
        {
            return await _dbSet
                        .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                        .Where(u => u.UserType == userType)
                        .ToListAsync();
        }
    }
}
