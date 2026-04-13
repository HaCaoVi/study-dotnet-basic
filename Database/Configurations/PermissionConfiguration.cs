using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using project_basic.Entities;

namespace project_basic.Database.Configurations;

public class PermissionConfiguration: IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Path)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Method)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(255);

        builder.HasIndex(p => new { p.Path, p.Method })
            .IsUnique();
    }
}