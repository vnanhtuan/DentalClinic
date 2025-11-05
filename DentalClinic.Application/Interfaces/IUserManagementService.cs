using DentalClinic.Application.DTOs;

namespace DentalClinic.Application.Interfaces
{
    public interface IUserManagementService
    {
        Task<int> RegisterPatientAsync(RegisterPatientDto request);
        Task<int> RegisterStaffAsync(RegisterStaffDto request);
    }
}
