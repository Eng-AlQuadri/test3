using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class MarkConfigurations : IEntityTypeConfiguration<Mark>
{
    public void Configure(EntityTypeBuilder<Mark> builder)
    {
        builder
            .HasOne(x => x.Subject)
            .WithMany(x => x.Marks)
            .HasForeignKey(x => x.SubjectId)
            .IsRequired();

        builder
            .HasOne(x => x.Student)
            .WithMany(x => x.Marks)
            .HasForeignKey(x => x.StudentId)
            .IsRequired();

        builder
            .ToTable("Marks");
    }
}
