using DentalClinic.Application.Modules.Branches.DTOs;
using DentalClinic.Application.Modules.Roles.DTOs;

namespace DentalClinic.Application.Modules.Staffs.DTOs
{
    public class StaffInitDto
    {
        public List<RoleDto> Roles { get; set; } = [];
        public List<BranchDto> Branches { get; set; } = [];
    }
}
