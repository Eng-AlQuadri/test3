using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder
            .HasOne(x => x.Sender)
            .WithMany(x => x.MessagesSent)
            .HasForeignKey(x => x.SenderId)
            .IsRequired();

        builder
            .HasOne(x => x.Recipient)
            .WithMany(x => x.MessagesReceived)
            .HasForeignKey(x => x.RecipientId)
            .IsRequired(false);

        builder
            .HasOne(x => x.Group)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.GroupId)
            .IsRequired(false);

        builder
            .Property(x => x.MessageSent).HasConversion(
                x => x.ToUniversalTime(),
                x => DateTime.SpecifyKind(x, DateTimeKind.Utc)
            );

        
        builder
            .ToTable("Messages");
    }
}
