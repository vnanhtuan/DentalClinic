using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Infrastructure.Repositories
{
    public class UserRepository: Repository<User>, IUserRepository
    {
        public UserRepository(DentalClinicDbContext dbContext) : base(dbContext)
        {
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
