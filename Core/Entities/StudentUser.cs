namespace Core.Entities;

public class StudentUser : BaseEntity
{
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;
    public ICollection<GroupStudents> GroupStudents { get; set; } = [];
    public ICollection<Mark> Marks { get; set; } = [];
    public ICollection<SubjectStudents> SubjectStudents { get; set; } = [];
}
