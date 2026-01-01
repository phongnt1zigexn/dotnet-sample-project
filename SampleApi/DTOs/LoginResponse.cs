namespace SampleApi.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = default!;
    public string TokenType { get; set; } = "Bearer";
    public int ExpiresIn { get; set; }
    public UserDto User { get; set; } = default!;
}
