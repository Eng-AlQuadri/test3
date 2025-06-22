using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder
            .HasOne(x => x.AppUser)
            .WithMany(x => x.Photos)
            .HasForeignKey(x => x.AppUserId)
            .IsRequired();

        builder
            .ToTable("Photos");
    }
}
