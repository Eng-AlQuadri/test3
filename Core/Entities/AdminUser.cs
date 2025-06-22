namespace Core.Entities;

public class AdminUser : BaseEntity
{
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;
}
