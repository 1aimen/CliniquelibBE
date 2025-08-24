using System.ComponentModel.DataAnnotations;

namespace Cliniquelib_BE.DTOs.Auth
{
    public class SigninRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
