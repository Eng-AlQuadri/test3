using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class CreateStudentDto
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string UserName { get; set; } = null!;
}
