using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalClinic.Application.DTOs
{
    public class LoginResponseDto
    {
        public string Status { get; set; } = "";
        public string Message { get; set; } = "";
        public string Token { get; set; }
        public string FullName { get; set; }
        public List<BranchSimpleDto> AvailableAssignments { get; set; } = [];
        public List<string> Roles { get; set; } = [];
    }

    public class BranchSimpleDto
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = null!;
    }
}
