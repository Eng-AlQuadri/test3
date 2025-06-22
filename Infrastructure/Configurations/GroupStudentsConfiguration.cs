using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class GroupStudentsConfiguration : IEntityTypeConfiguration<GroupStudents>
{
    public void Configure(EntityTypeBuilder<GroupStudents> builder)
    {
        builder
            .HasOne(x => x.Student)
            .WithMany(x => x.GroupStudents)
            .HasForeignKey(x => x.StudentId)
            .IsRequired();

        builder
            .HasOne(x => x.Group)
            .WithMany(x => x.GroupStudents)
            .HasForeignKey(x => x.GroupId)
            .IsRequired();

        builder
            .ToTable("GroupStudents");
    }
}
