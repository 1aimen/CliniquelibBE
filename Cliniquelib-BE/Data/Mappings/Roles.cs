using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cliniquelib_BE.Models;

namespace Cliniquelib_BE.Data.Mappings
{
    public class RoleMap : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Map to "roles" table
            builder.ToTable("roles");

            // Primary key
            builder.HasKey(r => r.Id);

            // Column mappings
            builder.Property(r => r.Id)
                   .HasColumnName("id")
                    .HasColumnType("uuid")
                   .IsRequired();

            builder.Property(r => r.Name)
                   .HasColumnName("name")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(r => r.Description)
                   .HasColumnName("description")
                   .HasMaxLength(500)
                   .IsRequired(false);

            // Relationships
            builder.HasMany(r => r.UserRoles)
                   .WithOne(ur => ur.Role)
                   .HasForeignKey(ur => ur.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
