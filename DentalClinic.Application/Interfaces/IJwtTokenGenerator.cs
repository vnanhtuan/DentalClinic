using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user, List<UserBranchMapping> assignments);
    }
}
