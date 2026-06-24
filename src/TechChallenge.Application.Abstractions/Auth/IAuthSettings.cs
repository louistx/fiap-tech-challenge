namespace TechChallenge.Application.Abstractions.Auth
{
    public interface IAuthSettings
    {
        int RefreshTokenDays { get; }
        int RefreshSessionMaxDays { get; }
        int RefreshOverlapSeconds { get; }
    }
}
