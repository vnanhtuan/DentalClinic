using DentalClinic.Domain.Entities;

namespace DentalClinic.Domain.Interfaces
{
    public interface IStaffRepository: IRepository<StaffDetail>
    {
        Task<StaffDetail?> GetByUsernameAsync(string username);
    }
}
