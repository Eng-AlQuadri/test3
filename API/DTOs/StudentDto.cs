namespace API.DTOs;

public class StudentDto
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public ICollection<PhotoDto> Photos { get; set; } = [];
}
