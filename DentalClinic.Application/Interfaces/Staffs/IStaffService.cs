using DentalClinic.Application.DTOs.Common;
using DentalClinic.Application.DTOs.Staffs;

namespace DentalClinic.Application.Interfaces.Staffs
{
    public interface IStaffService
    {
        Task<PagingResponse<StaffDto>> GetStaffPaginatedAsync(PagingParams pagingParams);
        Task<IEnumerable<StaffDto>> GetAllStaffAsync();
        Task<StaffDto?> GetStaffByIdAsync(int id);
        Task<int> CreateStaffAsync(StaffCreateDto staffCreateDto);
        Task UpdateStaffAsync(int id, StaffUpdateDto staffUpdateDto);
        Task DeleteStaffAsync(int id);
    }
}
