using Cliniquelib_BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cliniquelib_BE.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("user_roles");

            // Composite primary key
            builder.HasKey(ur => new { ur.UserId, ur.ClinicId, ur.RoleId });

            builder.Property(ur => ur.UserId).HasColumnName("user_id").HasColumnType("uuid");
            builder.Property(ur => ur.ClinicId).HasColumnName("clinic_id").HasColumnType("uuid");
            builder.Property(ur => ur.RoleId).HasColumnName("role_id").HasColumnType("uuid");

            // Relationships
            builder.HasOne(ur => ur.User)
                   .WithMany(u => u.UserRoles)
                   .HasForeignKey(ur => ur.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.Role)
                   .WithMany(r => r.UserRoles)
                   .HasForeignKey(ur => ur.RoleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
