namespace Core.Entities;

public class SubjectStudents : BaseEntity
{
    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = null!;
    public int StudentId { get; set; }
    public StudentUser Student { get; set; } = null!;
}
