using Cliniquelib_BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using Cliniquelib_BE.Models.Enums;
using System.Reflection.Emit;

namespace Cliniquelib_BE.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).HasColumnName("id");
            builder.Property(u => u.OrganizationId).HasColumnName("organization_id").IsRequired(false);
            builder.Property(u => u.Email).HasColumnName("email");
            builder.Property(u => u.FirstName).HasColumnName("first_name");
            builder.Property(u => u.LastName).HasColumnName("last_name");
            builder.Property(u => u.PasswordHash).HasColumnName("password_hash");
            builder.Property(u => u.Dob).HasColumnName("dob");
            builder.Property(u => u.Phone).HasColumnName("phone");
            builder.Property(u => u.Locale).HasColumnName("locale");
            builder.Property(u => u.IsActive).HasColumnName("is_active");
            builder.Property(u => u.CreatedAt).HasColumnName("created_at");
            builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");

            builder.Property(u => u.Sex)
                   .HasColumnName("sex")
                   .HasColumnType("sex_at_birth") // PostgreSQL enum type
                   .IsRequired();

            // JSONB converter + comparer for Meta & MfaMethods
            var dictionaryConverter = new ValueConverter<Dictionary<string, object>, string>(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                v => string.IsNullOrEmpty(v) ? new Dictionary<string, object>() : JsonSerializer.Deserialize<Dictionary<string, object>>(v, new JsonSerializerOptions())!
            );

            var dictionaryComparer = new ValueComparer<Dictionary<string, object>>(
                (c1, c2) => JsonSerializer.Serialize(c1, new JsonSerializerOptions()) == JsonSerializer.Serialize(c2, new JsonSerializerOptions()),
                c => c == null ? 0 : JsonSerializer.Serialize(c, new JsonSerializerOptions()).GetHashCode(),
                c => c == null ? new Dictionary<string, object>() : JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(c, new JsonSerializerOptions()), new JsonSerializerOptions())!
            );
            builder.Property(u => u.Sex)
       .HasColumnName("sex")
       .HasConversion(
            v => v.ToString(),  // Enum -> string
            v => Enum.Parse<Sex>(v) // string -> Enum
       )
       .HasColumnType("text")
       .IsRequired();

            builder.Property(u => u.Meta)
                   .HasColumnName("meta")
                   .HasColumnType("jsonb")
                   .HasConversion(dictionaryConverter)
                   .Metadata.SetValueComparer(dictionaryComparer);

            builder.Property(u => u.MfaMethods)
                   .HasColumnName("mfa_methods")
                   .HasColumnType("jsonb")
                   .HasConversion(dictionaryConverter)
                   .Metadata.SetValueComparer(dictionaryComparer);
        }
    }
}
