using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class UpdateStudentDto
{
    [Required]
    public int UpdateStudentId { get; set; }

    [Required]
    public string UserName { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;
}
