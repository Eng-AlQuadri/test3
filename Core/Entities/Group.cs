namespace Core.Entities;

public class Group : BaseEntity
{
    public Group() {}
    public Group(string groupName) 
    {
        Name = groupName;
    }
    public string Name { get; set; } = null!;
    public ICollection<GroupStudents> GroupStudents { get; set; } = [];
    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<Connection> Connections { get; set; } = [];
}
