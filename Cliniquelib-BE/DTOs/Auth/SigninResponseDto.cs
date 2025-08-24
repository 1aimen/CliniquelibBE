namespace Cliniquelib_BE.DTOs.Auth
{
    public class SigninResponseDto
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public UserDto User { get; set; }
    }
}
