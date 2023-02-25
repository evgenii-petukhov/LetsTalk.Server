namespace LetsTalk.Server.Models.Authentication;

public class LoginResponseDto
{
    public bool Success { get; set; }

    public string? Token { get; set; }
}

