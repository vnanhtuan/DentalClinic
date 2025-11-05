using DentalClinic.Application.DTOs;

namespace DentalClinic.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    }
}
