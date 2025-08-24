using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Cliniquelib_BE.Models
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid? ClinicId { get; set; }
        public Guid RoleId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(ClinicId))]
        public virtual Clinic? Clinic { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual Role Role { get; set; } = null!;
    }
}
