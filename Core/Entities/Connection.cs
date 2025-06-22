namespace Core.Entities;

public class Connection : BaseEntity
{
    public Connection() {}
    public Connection(string connectionId, int userId)
    {
        ConnectionId = connectionId;
        UserId = userId;
    }
    public string ConnectionId { get; set; } = null!;
    public int UserId { get; set; }
}
