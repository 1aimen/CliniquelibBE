using Cliniquelib_BE.DTOs.Auth;

namespace Cliniquelib_BE.Services
{
    public interface IAuthService
    {
        Task<SigninResponseDto> SignInAsync(SigninRequestDto request);
        Task<SignupResponseDto> SignUpAsync(SignupRequestDto request);
        Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task SignOutAsync(Guid userId);
        Task<Guid?> GetUserIdByRefreshTokenAsync(string refreshToken);
    }
}
