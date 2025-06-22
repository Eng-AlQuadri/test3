using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateSubjectDto
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public int MinMark { get; set; }
}
