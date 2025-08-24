using System.ComponentModel.DataAnnotations;

namespace Cliniquelib_BE.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
