using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Cliniquelib_BE.Models;

namespace Cliniquelib_BE.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens"); // <-- match the DB

            builder.HasKey(rt => rt.Token);

            builder.Property(rt => rt.Token)
                   .HasColumnName("token")
                   .HasColumnType("uuid");

            builder.Property(rt => rt.UserId)
                   .HasColumnName("user_id")
                   .HasColumnType("uuid");

            builder.Property(rt => rt.ExpiresAt)
                   .HasColumnName("expires_at");

            builder.Property(rt => rt.CreatedAt)
                   .HasColumnName("created_at");

            builder.HasOne(rt => rt.User)
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(rt => rt.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
