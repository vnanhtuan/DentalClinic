using DentalClinic.Domain.Entities;

namespace DentalClinic.Domain.Interfaces
{
    public interface IUserRepository: IRepository<User>
    {
        Task<User?> LoginByUsernameAsync(string username);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<IEnumerable<User>> GetAllWithRolesAsync();
        Task<User?> GetByIdWithRolesAsync(int id);
        Task<IEnumerable<User>> GetUsersByUserTypeAsync(string userType);
        Task<(List<User> Users, int TotalCount)> GetStaffPaginatedAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            string? sortBy,
            string? sortDirection,
            List<int>? roleIds);
    }
}
