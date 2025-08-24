using System.ComponentModel.DataAnnotations;

namespace Cliniquelib_BE.DTOs.Auth
{
    public class SignupRequestDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Phone { get; set; }
    }   
}
