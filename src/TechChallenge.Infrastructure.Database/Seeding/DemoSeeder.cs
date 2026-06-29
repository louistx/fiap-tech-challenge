using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechChallenge.Application.Abstractions.Auth;
using TechChallenge.Domain.Entities;
using TechChallenge.Domain.Enums;
using TechChallenge.Infrastructure.Database.Context;

namespace TechChallenge.Infrastructure.Database.Seeding;

internal static class DemoSeeder
{
    private static readonly Guid ClienteId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000001");
    private static readonly Guid EnderecoClienteId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000002");
    private static readonly Guid VendedorId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000003");
    private static readonly Guid EnderecoVendedorId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000004");
    private static readonly Guid MecanicoId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000005");
    private static readonly Guid EnderecoMecanicoId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000006");
    private static readonly Guid AdminId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000007");
    private static readonly Guid VeiculoId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000008");
    private static readonly Guid ServicoRevisaoId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000009");
    private static readonly Guid ServicoDiagnosticoId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000010");
    private static readonly Guid ProdutoFiltroId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000011");
    private static readonly Guid ProdutoPastilhaId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000012");
    private static readonly Guid UsuarioAdminId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000013");
    private static readonly Guid UsuarioVendedorId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000014");
    private static readonly Guid UsuarioMecanicoId = Guid.Parse("aaaaaaaa-0000-0000-0000-000000000015");

    public static async Task SeedAsync(IServiceProvider provider, ApplicationDbContext context)
    {
        var hasher = provider.GetRequiredService<IPasswordHasher>();

        await AddRangeIfMissingAsync(context, context.Endereco, Enderecos());
        await AddClienteIfMissingAsync(context);
        await AddRangeIfMissingAsync(context, context.Funcionario, Funcionarios());
        await AddRangeIfMissingAsync(context, context.Veiculo, Veiculos());
        await AddRangeIfMissingAsync(context, context.Servico, Servicos());
        await AddRangeIfMissingAsync(context, context.Produto, Produtos());
        await AddUsuariosIfMissingAsync(context, hasher);

        await context.SaveChangesAsync();
    }

    private static IReadOnlyCollection<Endereco> Enderecos() =>
    [
        new()
        {
            Id = EnderecoClienteId,
            Logradouro = "Rua Demo Cliente",
            Complemento = "Apto 101",
            Numero = "100",
            Bairro = "Centro",
            Cidade = "Sao Paulo",
            Estado = "SP",
            Cep = "01001000"
        },
        new()
        {
            Id = EnderecoVendedorId,
            Logradouro = "Rua Demo Oficina",
            Complemento = "Balcao",
            Numero = "200",
            Bairro = "Oficinas",
            Cidade = "Sao Paulo",
            Estado = "SP",
            Cep = "02002000"
        },
        new()
        {
            Id = EnderecoMecanicoId,
            Logradouro = "Rua Demo Oficina",
            Complemento = "Box 3",
            Numero = "200",
            Bairro = "Oficinas",
            Cidade = "Sao Paulo",
            Estado = "SP",
            Cep = "02002000"
        }
    ];

    private static Cliente Cliente() => new()
    {
        Id = ClienteId,
        Nome = "Cliente Demo",
        TipoDocumento = TipoDocumento.Cpf,
        Documento = "12345678909",
        EnderecoId = EnderecoClienteId
    };

    private static IReadOnlyCollection<Funcionario> Funcionarios() =>
    [
        new()
        {
            Id = VendedorId,
            Nome = "Vendedor Demo",
            Cpf = "12345678909",
            Rg = "111111111",
            TipoFuncionario = TipoFuncionario.Vendedor,
            EnderecoId = EnderecoVendedorId
        },
        new()
        {
            Id = MecanicoId,
            Nome = "Mecanico Demo",
            Cpf = "98765432100",
            Rg = "222222222",
            TipoFuncionario = TipoFuncionario.Mecanico,
            EnderecoId = EnderecoMecanicoId
        },
        new()
        {
            Id = AdminId,
            Nome = "Administrador Demo",
            Cpf = "11144477735",
            Rg = "333333333",
            TipoFuncionario = TipoFuncionario.Administrador,
            EnderecoId = EnderecoVendedorId
        }
    ];

    private static IReadOnlyCollection<Veiculo> Veiculos() =>
    [
        new()
        {
            Id = VeiculoId,
            Tipo = TipoVeiculo.Carro,
            Placa = "DEM1O23",
            Modelo = "Civic",
            Marca = "Honda",
            Cor = "Prata",
            Ano = 2020,
            Quilometragem = 45200,
            Valor = 98000,
            ClienteId = ClienteId
        }
    ];

    private static IReadOnlyCollection<Servico> Servicos() =>
    [
        new()
        {
            Id = ServicoRevisaoId,
            Descricao = "Revisao preventiva demo",
            Valor = 350
        },
        new()
        {
            Id = ServicoDiagnosticoId,
            Descricao = "Diagnostico eletrico demo",
            Valor = 180
        }
    ];

    private static IReadOnlyCollection<Produto> Produtos() =>
    [
        new()
        {
            Id = ProdutoFiltroId,
            Descricao = "Filtro de oleo demo",
            Valor = 65,
            Quantidade = 10
        },
        new()
        {
            Id = ProdutoPastilhaId,
            Descricao = "Pastilha de freio demo",
            Valor = 220,
            Quantidade = 1
        }
    ];

    private static IReadOnlyCollection<Usuario> Usuarios(IPasswordHasher hasher) =>
    [
        new()
        {
            Id = UsuarioAdminId,
            Login = "admin.demo",
            PasswordHash = hasher.Hash("Demo@123"),
            TipoUsuario = TipoUsuario.Administrador,
            FuncionarioId = AdminId,
            Ativo = true
        },
        new()
        {
            Id = UsuarioVendedorId,
            Login = "vendedor.demo",
            PasswordHash = hasher.Hash("Demo@123"),
            TipoUsuario = TipoUsuario.Vendedor,
            FuncionarioId = VendedorId,
            Ativo = true
        },
        new()
        {
            Id = UsuarioMecanicoId,
            Login = "mecanico.demo",
            PasswordHash = hasher.Hash("Demo@123"),
            TipoUsuario = TipoUsuario.Mecanico,
            FuncionarioId = MecanicoId,
            Ativo = true
        }
    ];

    private static async Task AddClienteIfMissingAsync(ApplicationDbContext context)
    {
        var cliente = Cliente();
        var exists = await context.Cliente.AnyAsync(c => c.Id == cliente.Id || c.Documento == cliente.Documento);

        if (!exists)
            context.Cliente.Add(cliente);
    }

    private static async Task AddUsuariosIfMissingAsync(ApplicationDbContext context, IPasswordHasher hasher)
    {
        foreach (var usuario in Usuarios(hasher))
        {
            var exists = await context.Usuario.AnyAsync(u => u.Id == usuario.Id || u.Login == usuario.Login);

            if (!exists)
                context.Usuario.Add(usuario);
        }
    }

    private static async Task AddRangeIfMissingAsync<TEntity>(
        ApplicationDbContext context,
        DbSet<TEntity> dbSet,
        IEnumerable<TEntity> entities)
        where TEntity : class
    {
        foreach (var entity in entities)
        {
            var id = context.Entry(entity).Property<Guid>("Id").CurrentValue;
            var exists = await dbSet.AnyAsync(e => EF.Property<Guid>(e, "Id") == id);

            if (!exists)
                dbSet.Add(entity);
        }
    }
}
