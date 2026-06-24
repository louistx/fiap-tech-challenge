using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.Auth.Refresh;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.Auth;

public class RefreshServiceTests
{
    private readonly Mock<IRefreshTokenRepository> _refresh = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly Mock<IAuthSettings> _settings = new();

    private RefreshService CriarService()
    {
        _settings.SetupGet(s => s.RefreshTokenDays).Returns(7);
        _settings.SetupGet(s => s.RefreshOverlapSeconds).Returns(0);
        return new RefreshService(_refresh.Object, _tokens.Object, _settings.Object);
    }

    [Fact]
    public void DeveLancarQuandoTokenNaoExiste()
    {
        _tokens.Setup(t => t.HashRefreshToken("cru")).Returns("hash");
        _refresh.Setup(r => r.GetByHashAsync("hash")).ReturnsAsync((RefreshToken?)null);

        var acao = () => CriarService().Refresh(new RefreshCommand { RefreshToken = "cru" });

        acao.Should().Throw<UnauthorizedAccessException>();
    }

    [Fact]
    public void DeveRotacionarQuandoTokenAtivo()
    {
        var usuario = new Usuario { Id = Guid.NewGuid(), Ativo = true, TipoUsuario = eTipoUsuario.Vendedor };
        var agora = DateTime.UtcNow;
        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UsuarioId = usuario.Id,
            Usuario = usuario,
            SessaoId = Guid.NewGuid(),
            CriadoEm = agora.AddMinutes(-1),
            ExpiraEm = agora.AddDays(7),
            SessaoExpiraEm = agora.AddDays(30)
        };
        _tokens.Setup(t => t.HashRefreshToken("cru")).Returns("hash");
        _tokens.Setup(t => t.HashRefreshToken("novo")).Returns("novo-hash");
        _refresh.Setup(r => r.GetByHashAsync("hash")).ReturnsAsync(token);
        _tokens.Setup(t => t.GerarAccessToken(usuario, token.SessaoId))
            .Returns(new AccessTokenResult("jwt", agora.AddMinutes(15)));
        _tokens.Setup(t => t.GerarRefreshToken()).Returns("novo");
        _refresh.Setup(r => r.AddAsync(It.IsAny<RefreshToken>())).ReturnsAsync((RefreshToken rt) => rt);
        _refresh.Setup(r => r.UpdateAsync(It.IsAny<RefreshToken>())).ReturnsAsync((RefreshToken rt) => rt);

        var resultado = CriarService().Refresh(new RefreshCommand { RefreshToken = "cru" });

        resultado.RefreshToken.Should().Be("novo");
        token.RevogadoEm.Should().NotBeNull();
        token.MotivoRevogacao.Should().Be("rotacionado");
        _refresh.Verify(r => r.AddAsync(It.IsAny<RefreshToken>()), Times.Once);
    }

    [Fact]
    public void DeveDetectarReusoERevogarSessao()
    {
        var agora = DateTime.UtcNow;
        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UsuarioId = Guid.NewGuid(),
            SessaoId = Guid.NewGuid(),
            ExpiraEm = agora.AddDays(7),
            SessaoExpiraEm = agora.AddDays(30),
            RevogadoEm = agora.AddMinutes(-5),     // já rotacionado, fora do overlap (0s)
            SubstituidoPorId = Guid.NewGuid(),
            MotivoRevogacao = "rotacionado"
        };
        _tokens.Setup(t => t.HashRefreshToken("cru")).Returns("hash");
        _refresh.Setup(r => r.GetByHashAsync("hash")).ReturnsAsync(token);

        var acao = () => CriarService().Refresh(new RefreshCommand { RefreshToken = "cru" });

        acao.Should().Throw<UnauthorizedAccessException>();
        _refresh.Verify(r => r.RevogarSessaoAsync(token.SessaoId, "reuso-detectado", It.IsAny<DateTime>()), Times.Once);
    }
}
