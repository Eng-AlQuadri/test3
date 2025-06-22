namespace Core.Entities;

public class Subject : BaseEntity
{
    public required string Name { get; set; }
    public int MinMark { get; set; }
    public ICollection<Mark> Marks { get; set; } = [];
    public ICollection<SubjectStudents> SubjectStudents { get; set; } = [];
}
