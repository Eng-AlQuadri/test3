using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class SetMarkDto
{
    [Required]
    public string SubjectName { get; set; } = null!;
    
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int GainedMark { get; set; }
}
