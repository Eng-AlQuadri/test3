namespace API.DTOs;

public class UserDto
{
    public string Token { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string? PhotoUrl { get; set; }
}
