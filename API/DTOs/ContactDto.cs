namespace API.DTOs;

public class ContactDto
{
    public int ContactId { get; set; }
    public string? PhotoUrl { get; set; } 
    public string UserName { get; set; } = null!;
    public int UnreadMessages { get; set; }
}
