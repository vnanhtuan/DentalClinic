using DentalClinic.Domain.Entities;

namespace DentalClinic.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        Task<string> GenerateTokenAsync(User user);
    }
}
