namespace TechChallenge.Application.Features.Auth.Login;

public class LoginCommand
{
    public string Login { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public string? Ip { get; set; }
}
