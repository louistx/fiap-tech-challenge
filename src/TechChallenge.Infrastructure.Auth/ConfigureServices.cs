using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.Auth.Login;
using TechChallenge.Application.Features.Auth.Logout;
using TechChallenge.Application.Features.Auth.Refresh;
using TechChallenge.Application.Features.Auth.Sessoes;
using TechChallenge.Application.Features.Auth.TrocarSenha;
using TechChallenge.Application.Features.Usuarios.AlterarStatus;
using TechChallenge.Application.Features.Usuarios.AlterarTipo;
using TechChallenge.Application.Features.Usuarios.CriarUsuario;
using TechChallenge.Application.Features.Usuarios.DesvincularFuncionario;
using TechChallenge.Application.Features.Usuarios.ListarUsuarios;
using TechChallenge.Application.Features.Usuarios.ObterUsuario;
using TechChallenge.Application.Features.Usuarios.ResetarSenha;
using TechChallenge.Application.Features.Usuarios.VincularFuncionario;
using TechChallenge.Infrastructure.Database.Repositories;
using TechChallenge.Infrastructure.Auth;

namespace TechChallenge.Infrastructure.Auth
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<JwtOptions>()
                .Bind(configuration.GetSection(JwtOptions.SectionName))
                .Validate(o => !string.IsNullOrWhiteSpace(o.SecretKey) && o.SecretKey.Length >= 32,
                    "Jwt:SecretKey ausente ou com menos de 32 caracteres.")
                .ValidateOnStart();

            services.AddHttpContextAccessor();

            services.AddSingleton<IAuthSettings, AuthSettings>();
            services.AddSingleton<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher, Pbkdf2PasswordHasher>();
            services.AddScoped<ICurrentUser, CurrentUser>();

            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            services.AddScoped<LoginService>();
            services.AddScoped<RefreshService>();
            services.AddScoped<TrocarSenhaService>();
            services.AddScoped<LogoutService>();
            services.AddScoped<ListarSessoesService>();
            services.AddScoped<RevogarSessaoService>();

            services.AddScoped<CriarUsuarioService>();
            services.AddScoped<ListarUsuariosService>();
            services.AddScoped<ObterUsuarioService>();
            services.AddScoped<AlterarTipoService>();
            services.AddScoped<VincularFuncionarioService>();
            services.AddScoped<DesvincularFuncionarioService>();
            services.AddScoped<AlterarStatusUsuarioService>();
            services.AddScoped<ResetarSenhaService>();

            var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(string.IsNullOrWhiteSpace(jwt.SecretKey)
                    ? new string('0', 32)
                    : jwt.SecretKey));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromSeconds(30),
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = signingKey,
                        NameClaimType = TokenService.ClaimSub,
                        RoleClaimType = TokenService.ClaimRole
                    };
                });

            services.AddAuthorizationBuilder()
                // Defesa: todo endpoint exige autenticação por padrão; anônimos usam .AllowAnonymous().
                .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
                .AddPolicy("AdminOnly", policy => policy.RequireRole("Administrador"))
                .AddPolicy("AdminOuVendedor", policy => policy.RequireRole("Administrador", "Vendedor"))
                .AddPolicy("MecanicoOuVendedor", policy => policy.RequireRole("Administrador", "Mecanico", "Vendedor"))
                .AddPolicy("Mecanico", policy => policy.RequireRole("Mecanico"));

            return services;
        }
    }
}
