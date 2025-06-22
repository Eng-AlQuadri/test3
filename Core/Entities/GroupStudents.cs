namespace Core.Entities;

public class GroupStudents : BaseEntity
{
    public int GroupId { get; set; }
    public Group Group { get; set; } = null!;
    public int StudentId { get; set;}
    public StudentUser Student { get; set; } = null!;
}
