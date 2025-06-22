namespace API.DTOs;

public class StudentSubjectDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = null!;
    public int MinMark { get; set; } 
    public int? GainedMark { get; set; }
}
