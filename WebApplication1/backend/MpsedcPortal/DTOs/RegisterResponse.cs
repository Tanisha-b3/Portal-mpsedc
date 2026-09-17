namespace MpsedcPortal.DTOs;

public class RegisterResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ProfileImageName { get; set; }
    public List<string>? Errors { get; set; }
}
