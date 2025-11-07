using DentalClinic.Domain.Entities;

namespace DentalClinic.Domain.Interfaces
{
    public interface IUserRepository: IRepository<User>
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<IEnumerable<User>> GetAllWithRolesAsync();
        Task<User?> GetByIdWithRolesAsync(int id);
        Task<IEnumerable<User>> GetUsersByUserTypeAsync(string userType);
    }
}
