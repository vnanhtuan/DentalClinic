namespace DentalClinic.Application.Providers
{
    public interface ICurrentUserProvider
    {
        // User Identity
        int UserId { get; }
        int BranchId { get; }
        string Username { get; }
        string FullName { get; }
        string Email { get; }
        bool IsAuthenticated { get; }

        // User Type Checks
        bool IsSuperAdmin { get; }
        bool IsAdmin { get; }
        bool IsStaff { get; }
        bool IsPatient { get; }
        string UserTypeCode { get; }

        // Role Information
        IReadOnlyList<string> Roles { get; }
        bool HasRole(string roleName);
        bool HasAnyRole(params string[] roleNames);
        bool HasAllRoles(params string[] roleNames);

        // Branch Access
        IReadOnlyList<int> AccessibleBranchIds { get; }
        bool HasAccessToBranch(int branchId);
        bool CanAccessAllBranches { get; }

        // Permissions
        IReadOnlyList<string> Permissions { get; }
        bool HasPermission(string permission);

        // Metadata
        string IpAddress { get; }
        DateTime? LoginTime { get; }

        // Utility
        T? GetClaim<T>(string claimType);
    }
}
