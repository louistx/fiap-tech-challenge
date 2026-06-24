using TechChallenge.Api.Models.Request;
using TechChallenge.Api.Models.Response;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Features.Auth.Login;
using TechChallenge.Application.Features.Auth.Logout;
using TechChallenge.Application.Features.Auth.Refresh;
using TechChallenge.Application.Features.Auth.Sessoes;
using TechChallenge.Application.Features.Auth.TrocarSenha;
using TechChallenge.Application.Features.Usuarios.ObterUsuario;

namespace TechChallenge.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth").WithTags("Auth");

        group.MapPost("/login", LoginAsync)
            .AllowAnonymous()
            .WithName("Login")
            .WithSummary("Autentica um usuário")
            .Produces<LoginResponse>()
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesValidationProblem();

        group.MapPost("/refresh", RefreshAsync)
            .AllowAnonymous()
            .WithName("RefreshToken")
            .WithSummary("Renova o access token a partir de um refresh token")
            .Produces<LoginResponse>()
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("/me", ObterUsuarioLogado)
            .RequireAuthorization()
            .WithName("ObterUsuarioLogado")
            .WithSummary("Retorna os dados do usuário autenticado")
            .Produces<UsuarioLogadoResponse>()
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("/logout", LogoutAsync)
            .RequireAuthorization()
            .WithName("Logout")
            .WithSummary("Revoga a sessão atual (claim sid do token)")
            .Produces(StatusCodes.Status204NoContent);

        group.MapPost("/logout-all", LogoutTodasAsync)
            .RequireAuthorization()
            .WithName("LogoutTodas")
            .WithSummary("Revoga todas as sessões do usuário autenticado")
            .Produces(StatusCodes.Status204NoContent);

        group.MapPatch("/senha", TrocarSenhaAsync)
            .RequireAuthorization()
            .WithName("TrocarSenha")
            .WithSummary("Troca a senha do usuário autenticado")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem();

        group.MapGet("/sessoes", ListarSessoes)
            .RequireAuthorization()
            .WithName("ListarSessoes")
            .WithSummary("Lista as sessões ativas do usuário autenticado")
            .Produces<List<SessaoResponse>>();

        group.MapDelete("/sessoes/{sessaoId}", RevogarSessao)
            .RequireAuthorization()
            .WithName("RevogarSessao")
            .WithSummary("Revoga uma sessão específica")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    private static IResult LoginAsync(LoginRequest request, LoginService service, HttpContext http)
    {
        var command = new LoginCommand
        {
            Login = request.Login,
            Senha = request.Senha,
            UserAgent = http.Request.Headers.UserAgent.ToString(),
            Ip = http.Connection.RemoteIpAddress?.ToString()
        };

        var resultado = service.Login(command);
        return Results.Ok(MapToResponse(resultado));
    }

    private static IResult RefreshAsync(RefreshRequest request, RefreshService service)
    {
        var resultado = service.Refresh(new RefreshCommand { RefreshToken = request.RefreshToken });
        return Results.Ok(MapToResponse(resultado));
    }

    private static IResult ObterUsuarioLogado(ICurrentUser currentUser, ObterUsuarioService service)
    {
        if (currentUser.UsuarioId is not { } usuarioId)
            return Results.Unauthorized();

        var usuario = service.ObterUsuario(new ObterUsuarioQuery { Id = usuarioId });
        return Results.Ok(new UsuarioLogadoResponse(
            usuario.Id,
            usuario.Login,
            usuario.TipoUsuario.ToString(),
            usuario.FuncionarioId));
    }

    private static IResult LogoutAsync(LogoutService service)
    {
        service.Logout();
        return Results.NoContent();
    }

    private static IResult LogoutTodasAsync(LogoutService service)
    {
        service.LogoutTodas();
        return Results.NoContent();
    }

    private static IResult TrocarSenhaAsync(TrocarSenhaRequest request, TrocarSenhaService service)
    {
        service.TrocarSenha(new TrocarSenhaCommand
        {
            SenhaAtual = request.SenhaAtual,
            NovaSenha = request.NovaSenha
        });
        return Results.NoContent();
    }

    private static IResult ListarSessoes(ListarSessoesService service)
    {
        var sessoes = service.ListarSessoes()
            .Select(s => new SessaoResponse(s.SessaoId, s.CriadoEm, s.ExpiraEm, s.UserAgent, s.IpCriacao))
            .ToList();
        return Results.Ok(sessoes);
    }

    private static IResult RevogarSessao(Guid sessaoId, RevogarSessaoService service)
    {
        service.RevogarSessao(sessaoId);
        return Results.NoContent();
    }

    private static LoginResponse MapToResponse(Application.Features.Auth.AuthResult resultado) =>
        new(resultado.AccessToken, resultado.ExpiraEm, resultado.RefreshToken, "Bearer");
}
