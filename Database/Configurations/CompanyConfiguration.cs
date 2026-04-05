using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using project_basic.Models;

namespace project_basic.Database.Configurations;

public class CompanyConfiguration: IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        
        builder.Property(c => c.Address).IsRequired().HasMaxLength(100);
        
        builder.HasQueryFilter(c => !c.IsDeleted);
        
        builder.HasOne(c => c.User)
            .WithOne(u => u.Company)
            .HasForeignKey<Company>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasIndex(c => c.UserId).IsUnique();
    }
}