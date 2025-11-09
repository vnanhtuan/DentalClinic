using DentalClinic.Application.Interfaces;
using DentalClinic.Domain.Common;
using DentalClinic.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DentalClinic.Infrastructure.Security
{
    public class JwtTokenGenerator: IJwtTokenGenerator
    {
        private readonly IConfiguration _configuration;
        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            var secretKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("UserType", user.UserType),
                new Claim(ClaimTypes.Role, user.UserType)
            };

            if (user.UserType == UserTypeCodes.Staff && user.StaffDetail != null)
            {
                claims.Add(new Claim("StaffRole", user.StaffDetail.RoleTitle ?? UserTypeCodes.Staff));
            }

            if (user.UserRoles != null)
            {
                foreach (var userRole in user.UserRoles)
                {
                    // Add roles (Ex: "Admin", "Manage")
                    // For [Authorize(Roles = "...")]
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.RoleName));
                }
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
