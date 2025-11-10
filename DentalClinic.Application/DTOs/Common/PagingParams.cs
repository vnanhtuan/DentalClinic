namespace DentalClinic.Application.DTOs.Common
{
    public class PagingParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? SortBy { get; set; }
        public string? SortDirection { get; set; } = "asc";
        public string? SearchTerm { get; set; }

        // New: filter by one or more RoleIds (model binding supports repeated query params: roleIds=1&roleIds=2)
        public List<int>? RoleIds { get; set; }
        public List<int>? BranchIds { get; set; }
    }
}
