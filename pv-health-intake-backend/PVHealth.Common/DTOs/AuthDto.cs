namespace PVHealth.Common.DTOs;

public class MicrosoftAuthRequest
{
    public string AccessToken {get;set;}=string.Empty;
}
public class AuthResponse
{
    public string Token {get;set;} = string.Empty;
    public UserDto User {get;set;} = null!;
    public bool RequiresMfa {get;set;} = false;
}
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? ProfilePicture { get; set; }
}