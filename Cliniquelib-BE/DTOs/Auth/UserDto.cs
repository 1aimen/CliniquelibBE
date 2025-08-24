using System;
using System.Collections.Generic;

namespace Cliniquelib_BE.DTOs.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<string> Roles { get; set; } = new List<string>();
    }
}
