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
    }
}
