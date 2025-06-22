using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> builder)
    {
        builder
            .HasOne(x => x.AppUser)
            .WithOne()
            .HasForeignKey<AdminUser>(x => x.AppUserId);

        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .ToTable("Admins");
    }
}
