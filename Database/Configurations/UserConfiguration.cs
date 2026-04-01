using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using project_basic.Models;

namespace project_basic.Database.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Password).IsRequired();
        builder.Property(u => u.Name).IsRequired().HasMaxLength(256);
        builder.Property(u => u.Address)
            .HasMaxLength(255)
            .HasDefaultValue(string.Empty);
    }
}