using FluentAssertions;
using Moq;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Application.Features.Usuarios.CriarUsuario;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;

namespace TechChallenge.Tests.Features.Usuarios;

public class CriarUsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarios = new();
    private readonly Mock<IFuncionarioRepository> _funcionarios = new();
    private readonly Mock<IPasswordHasher> _hasher = new();

    private CriarUsuarioService CriarService()
    {
        _hasher.Setup(h => h.Hash(It.IsAny<string>())).Returns("hash");
        return new CriarUsuarioService(_usuarios.Object, _funcionarios.Object, _hasher.Object,
            new CriarUsuarioCommandValidator());
    }

    private static CriarUsuarioCommand Comando(Guid? funcionarioId = null) => new()
    {
        Login = "novo.usuario",
        Senha = "Senha@123",
        TipoUsuario = eTipoUsuario.Vendedor,
        FuncionarioId = funcionarioId
    };

    [Fact]
    public void DeveCriarUsuarioQuandoValido()
    {
        _usuarios.Setup(r => r.ExisteLoginAsync("novo.usuario")).ReturnsAsync(false);
        Usuario? salvo = null;
        _usuarios.Setup(r => r.AddAsync(It.IsAny<Usuario>()))
            .Callback<Usuario>(u => salvo = u)
            .ReturnsAsync((Usuario u) => u);

        var id = CriarService().CriarUsuario(Comando());

        id.Should().NotBeEmpty();
        salvo.Should().NotBeNull();
        salvo!.PasswordHash.Should().Be("hash");
        salvo.TipoUsuario.Should().Be(eTipoUsuario.Vendedor);
    }

    [Fact]
    public void DeveImpedirLoginDuplicado()
    {
        _usuarios.Setup(r => r.ExisteLoginAsync("novo.usuario")).ReturnsAsync(true);

        var acao = () => CriarService().CriarUsuario(Comando());

        acao.Should().Throw<InvalidOperationException>();
        _usuarios.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
    }

    [Fact]
    public void DeveLancarQuandoFuncionarioVinculadoNaoExiste()
    {
        var funcionarioId = Guid.NewGuid();
        _usuarios.Setup(r => r.ExisteLoginAsync("novo.usuario")).ReturnsAsync(false);
        _funcionarios.Setup(r => r.GetByIdAsync(funcionarioId)).ReturnsAsync((Funcionario?)null);

        var acao = () => CriarService().CriarUsuario(Comando(funcionarioId));

        acao.Should().Throw<KeyNotFoundException>();
    }

    [Fact]
    public void DeveImpedirVinculoDuplicado()
    {
        var funcionarioId = Guid.NewGuid();
        _usuarios.Setup(r => r.ExisteLoginAsync("novo.usuario")).ReturnsAsync(false);
        _funcionarios.Setup(r => r.GetByIdAsync(funcionarioId)).ReturnsAsync(new Funcionario { Id = funcionarioId });
        _usuarios.Setup(r => r.ExisteVinculoFuncionarioAsync(funcionarioId)).ReturnsAsync(true);

        var acao = () => CriarService().CriarUsuario(Comando(funcionarioId));

        acao.Should().Throw<InvalidOperationException>();
    }
}
