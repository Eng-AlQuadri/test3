using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class SubjectStudentsConfiguration : IEntityTypeConfiguration<SubjectStudents>
{
    public void Configure(EntityTypeBuilder<SubjectStudents> builder)
    {
        builder
            .HasOne(x => x.Subject)
            .WithMany(x => x.SubjectStudents)
            .HasForeignKey(x => x.SubjectId)
            .IsRequired();

        builder
            .HasOne(x => x.Student)
            .WithMany(x => x.SubjectStudents)
            .HasForeignKey(x => x.StudentId)
            .IsRequired();

        builder
            .ToTable("SubjectStudents");
    }
}
