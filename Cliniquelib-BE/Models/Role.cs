using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cliniquelib_BE.Models
{
    public class Role
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty; // e.g., "Admin", "Doctor", "Patient"

        [MaxLength(500)]
        public string? Description { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
