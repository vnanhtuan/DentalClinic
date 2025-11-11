using DentalClinic.Application.DTOs.Common;

namespace DentalClinic.Application.Modules.Staffs.DTOs
{
    public class StaffPagingParams: BasePagingParams
    {
        public List<int>? RoleIds { get; set; }
        public List<int>? BranchIds { get; set; }
    }
}
