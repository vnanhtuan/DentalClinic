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
        private readonly IUserRepository _userRepository;
        private readonly IBranchRepository _branchRepository;
        public JwtTokenGenerator(IOptions<JwtSettings> jwtSettings,
            IUserRepository userRepository,
            IBranchRepository branchRepository)
        {
            _jwtSettings = jwtSettings.Value;
            _userRepository = userRepository;
            _branchRepository = branchRepository;
        }
        public async Task<string> GenerateTokenAsync(User user)
        {
            var claims = await BuildClaimAsync(user);
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

        private async Task<List<Claim>> BuildClaimAsync(User user)
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

            if (user.UserRoles != null)
            {
                foreach (var userRole in user.UserRoles)
                {
                    // Add roles (Ex: "Admin", "Manage")
                    // For [Authorize(Roles = "...")]
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
                    claims.Add(new Claim("role", userRole.Role.RoleName));
                }
            }

            if (user.UserType == UserTypeCodes.Staff)
            {
                var userBranches = await _branchRepository.GetBranchesByUserIdAsync(user.UserId);
                foreach (var branch in userBranches)
                {
                    claims.Add(new Claim("branchId", branch.BranchId.ToString()));
                }

                // Optional: Add branch names for display purposes
                //foreach (var branch in userBranches)
                //{
                //    claims.Add(new Claim("branchName", branch.BranchName));
                //}
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
