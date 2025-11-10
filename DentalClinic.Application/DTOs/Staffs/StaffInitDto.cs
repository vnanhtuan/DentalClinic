using DentalClinic.Application.DTOs.Branches;
using DentalClinic.Application.DTOs.Roles;

namespace DentalClinic.Application.DTOs.Staffs
{
    public class StaffInitDto
    {
        public List<RoleDto> Roles { get; set; } = [];
        public List<BranchDto> Branches { get; set; } = [];
    }
}
