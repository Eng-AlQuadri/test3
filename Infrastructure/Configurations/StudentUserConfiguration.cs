using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class StudentUserConfiguration : IEntityTypeConfiguration<StudentUser>
{
    public void Configure(EntityTypeBuilder<StudentUser> builder)
    {
        builder
            .HasOne(x => x.AppUser)
            .WithOne()
            .HasForeignKey<StudentUser>(x => x.AppUserId);
            
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever();

        builder
            .ToTable("Students");
    }
}
