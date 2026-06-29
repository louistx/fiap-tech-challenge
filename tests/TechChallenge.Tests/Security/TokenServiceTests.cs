using System;
using System.IdentityModel.Tokens.Jwt;
using FluentAssertions;
using Microsoft.Extensions.Options;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Infrastructure.Auth;

namespace TechChallenge.Tests.Security;

public class TokenServiceTests
{
    private static TokenService CriarService() => new(Options.Create(new JwtOptions
    {
        Issuer = "techchallenge-api",
        Audience = "techchallenge-clients",
        SecretKey = "secret-de-teste-com-mais-de-32-caracteres-000",
        AccessTokenMinutes = 15
    }));

    [Fact]
    public void AccessTokenDeveConterClaimsDeIdentidadeERole()
    {
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Login = "admin",
            TipoUsuario = TipoUsuario.Administrador
        };

        var resultado = CriarService().GerarAccessToken(usuario);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(resultado.Token);
        token.Claims.Should().Contain(c => c.Type == "sub" && c.Value == usuario.Id.ToString());
        token.Claims.Should().Contain(c => c.Type == "role" && c.Value == "Administrador");
        token.Claims.Should().Contain(c => c.Type == "name" && c.Value == "admin");
        token.Claims.Should().NotContain(c => c.Type == "sid");
        resultado.ExpiraEm.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void AccessTokenDeveIncluirFuncionarioIdQuandoVinculado()
    {
        var funcionarioId = Guid.NewGuid();
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Login = "mec",
            TipoUsuario = TipoUsuario.Mecanico,
            FuncionarioId = funcionarioId
        };

        var resultado = CriarService().GerarAccessToken(usuario);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(resultado.Token);
        token.Claims.Should().Contain(c => c.Type == "funcionarioId" && c.Value == funcionarioId.ToString());
    }

    [Fact]
    public void RefreshTokenEHashDevemSerConsistentes()
    {
        var service = CriarService();
        var cru = service.GerarRefreshToken();

        cru.Should().NotBeNullOrWhiteSpace();
        service.HashRefreshToken(cru).Should().Be(service.HashRefreshToken(cru));
        service.HashRefreshToken(cru).Should().NotBe(cru);
    }
}
