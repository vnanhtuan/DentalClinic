using DentalClinic.Application.Interfaces;
using DentalClinic.Domain.Common;
using DentalClinic.Domain.Entities;
using DentalClinic.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DentalClinic.Infrastructure.Security
{
    public class JwtTokenGenerator: IJwtTokenGenerator
    {
        private readonly JwtSettings _jwtSettings;
        public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings,
            IUserRepository userRepository,
            IBranchRepository branchRepository)
        {
            _jwtSettings = jwtSettings.Value;
        }
        public string GenerateToken(User user, List<UserBranchMapping> assignments)
        {
            var claims = BuildClaim(user, assignments);
            // Create token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private List<Claim> BuildClaim(User user, List<UserBranchMapping> assignments)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

                new Claim("auth_time", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new Claim("userTypeCode", user.UserType)
            };

            if (assignments != null && assignments.Any())
            {
                var primaryAssignment = assignments.First();

                claims.Add(new Claim("branchId", primaryAssignment.BranchId.ToString()));

                var roleNames = assignments.Select(a => a.Role.RoleName).Distinct();
                foreach (var roleName in roleNames)
                {
                    // Add roles (Ex: "Admin", "Manage")
                    // For [Authorize(Roles = "...")]
                    claims.Add(new Claim(ClaimTypes.Role, roleName));
                }
            }
            else if (user.UserType == UserTypeCodes.SuperAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, RoleConstants.Admin));
                claims.Add(new Claim(ClaimTypes.Role, RoleConstants.Manager));
                claims.Add(new Claim(ClaimTypes.Role, RoleConstants.Doctor));
                claims.Add(new Claim(ClaimTypes.Role, RoleConstants.Staff));
                claims.Add(new Claim(ClaimTypes.Role, RoleConstants.Nurse));
            }

            // Add permissions (if you have a permission system)
            //var permissions = await _userRepository.GetUserPermissionsAsync(user.UserId);
            //foreach (var permission in permissions)
            //{
            //    claims.Add(new Claim("permission", permission));
            //}

            return claims;
        }
    }
}
