namespace Cliniquelib_BE.DTOs.Auth
{
    public class RefreshTokenResponseDto
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
