namespace Core.Entities;

public class Photo : BaseEntity
{
    public required string Url { get; set; }
    public bool IsMain { get; set; }
    public required string PublicId { get; set; }
    public AppUser AppUser { get; set; } = null!;
    public int AppUserId { get; set; }
}
