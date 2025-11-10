using DentalClinic.Application.Providers;
using DentalClinic.Domain.Common;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DentalClinic.Infrastructure.Providers
{
    public class HttpContextCurrentUserProvider: ICurrentUserProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public HttpContextCurrentUserProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int UserId
        {
            get
            {
                var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                               ?? User?.FindFirst("sub")?.Value
                               ?? User?.FindFirst("userId")?.Value;
                return int.TryParse(userIdClaim, out var id) ? id : 0;
            }
        }

        public string Username =>
        User?.FindFirst(ClaimTypes.Name)?.Value
        ?? User?.FindFirst("username")?.Value
        ?? string.Empty;

        public string FullName =>
        User?.FindFirst("fullName")?.Value
        ?? User?.FindFirst(ClaimTypes.GivenName)?.Value
        ?? string.Empty;

        public string Email =>
            User?.FindFirst(ClaimTypes.Email)?.Value
            ?? User?.FindFirst("email")?.Value
            ?? string.Empty;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public string UserTypeCode =>
        User?.FindFirst("userTypeCode")?.Value
        ?? User?.FindFirst("userType")?.Value
        ?? string.Empty;

        public bool IsSuperAdmin =>
        UserTypeCode.Equals(UserTypeCodes.SuperAdmin, StringComparison.OrdinalIgnoreCase);

        public bool IsAdmin =>
        UserTypeCode.Equals(UserTypeCodes.Admin, StringComparison.OrdinalIgnoreCase)
        || IsSuperAdmin;

        public bool IsStaff =>
            UserTypeCode.Equals(UserTypeCodes.Staff, StringComparison.OrdinalIgnoreCase);

        public bool IsPatient =>
            UserTypeCode.Equals(UserTypeCodes.Patient, StringComparison.OrdinalIgnoreCase);

        public IReadOnlyList<string> Roles
        {
            get
            {
                var roleClaims = User?.FindAll(ClaimTypes.Role)
                    ?? User?.FindAll("role")
                    ?? Enumerable.Empty<Claim>();
                return roleClaims.Select(c => c.Value).ToList().AsReadOnly();
            }
        }

        public bool HasRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) return false;
            return Roles.Any(r => r.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }

        public bool HasAnyRole(params string[] roleNames)
        {
            if (roleNames == null || roleNames.Length == 0) return false;
            return Roles.Any(r => roleNames.Contains(r, StringComparer.OrdinalIgnoreCase));
        }

        public bool HasAllRoles(params string[] roleNames)
        {
            if (roleNames == null || roleNames.Length == 0) return false;
            return roleNames.All(rn => Roles.Contains(rn, StringComparer.OrdinalIgnoreCase));
        }

        // ==================== BRANCH ACCESS ====================
        public IReadOnlyList<int> AccessibleBranchIds
        {
            get
            {
                var branchClaims = User?.FindAll("branchId")
                    ?? Enumerable.Empty<Claim>();
                return branchClaims
                    .Select(c => int.TryParse(c.Value, out var bid) ? bid : 0)
                    .Where(bid => bid > 0)
                    .Distinct()
                    .ToList()
                    .AsReadOnly();
            }
        }

        public bool HasAccessToBranch(int branchId)
        {
            // Admins have access to all branches
            if (IsAdmin) return true;

            // Check if branch is in accessible list
            return AccessibleBranchIds.Contains(branchId);
        }

        public bool CanAccessAllBranches => IsAdmin || IsSuperAdmin;

        // ==================== PERMISSIONS ====================

        public IReadOnlyList<string> Permissions
        {
            get
            {
                var permClaims = User?.FindAll("permission")
                    ?? Enumerable.Empty<Claim>();
                return permClaims.Select(c => c.Value).ToList().AsReadOnly();
            }
        }

        public bool HasPermission(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission)) return false;

            // Admins have all permissions
            if (IsAdmin) return true;

            return Permissions.Any(p => p.Equals(permission, StringComparison.OrdinalIgnoreCase));
        }

        // ==================== METADATA ====================

        public string IpAddress =>
            _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
            ?? string.Empty;

        public DateTime? LoginTime
        {
            get
            {
                var authTimeClaim = User?.FindFirst("auth_time")?.Value;
                if (long.TryParse(authTimeClaim, out var unixTime))
                {
                    return DateTimeOffset.FromUnixTimeSeconds(unixTime).DateTime;
                }
                return null;
            }
        }

        // ==================== UTILITY METHODS ====================

        public T? GetClaim<T>(string claimType)
        {
            var claimValue = User?.FindFirst(claimType)?.Value;
            if (string.IsNullOrEmpty(claimValue)) return default;

            try
            {
                return (T)Convert.ChangeType(claimValue, typeof(T));
            }
            catch
            {
                return default;
            }
        }
    }
}
