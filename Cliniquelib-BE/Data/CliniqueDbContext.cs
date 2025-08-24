using Cliniquelib_BE.Configurations;
using Cliniquelib_BE.Data.Mappings;
using Cliniquelib_BE.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql;

public class CliniqueDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Role> Roles { get; set; }


    public CliniqueDbContext(DbContextOptions<CliniqueDbContext> options)
        : base(options)
    {
        // Ensure enum mapping globally
        NpgsqlConnection.GlobalTypeMapper.MapEnum<Sex>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            entityType.SetTableName(entityType.GetTableName()!.ToLower());
            foreach (var property in entityType.GetProperties())
            {
                property.SetColumnName(property.GetColumnName(StoreObjectIdentifier.Table(entityType.GetTableName()!, null))!.ToLower());
            }
        }

        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());

        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleMap());
        // Register enum with PostgreSQL
        modelBuilder.HasPostgresEnum<Sex>("sex_at_birth");
        modelBuilder.Entity<Role>().ToTable("roles");
        // Optional: table configuration
        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Sex).HasColumnType("sex_at_birth");

       

            // Role mapping
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles"); // table name in DB
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Id).HasColumnName("id");
                entity.Property(r => r.Name).HasColumnName("name");
                entity.Property(r => r.Description).HasColumnName("description");
            });
            // Map Role entity to table "roles" if not already done
            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("roles"); // ensure lowercase
                entity.HasKey(r => r.Id);
            });
            // JSONB conversion for dictionaries
            var dictionaryConverter = new ValueConverter<Dictionary<string, object>, string>(
                v => System.Text.Json.JsonSerializer.Serialize(v, new System.Text.Json.JsonSerializerOptions()),
                v => string.IsNullOrEmpty(v) ? new Dictionary<string, object>() : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, new System.Text.Json.JsonSerializerOptions())!
            );

            builder.Property(u => u.Meta)
                .HasColumnType("jsonb")
                .HasConversion(dictionaryConverter);

            builder.Property(u => u.MfaMethods)
                .HasColumnType("jsonb")
                .HasConversion(dictionaryConverter);
        });
    }
}
