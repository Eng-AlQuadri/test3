namespace Core.Entities;

public class Message : BaseEntity
{
    public int SenderId { get; set; }
    public required string SenderName { get; set; }
    public AppUser Sender { get; set; } = null!;
    public int? RecipientId { get; set; }
    public string? RecipientName { get; set; }
    public AppUser? Recipient { get; set; }
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
    public Group? Group { get; set; }
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; } 
    public DateTime MessageSent { get; set; } = DateTime.UtcNow;
}
