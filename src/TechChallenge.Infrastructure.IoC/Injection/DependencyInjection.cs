using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechChallenge.Application.Features.OS.AtribuirOS;
using TechChallenge.Application.Features.OS.CriarOS;
using TechChallenge.Application.Features.OS.ExcluirOS;
using TechChallenge.Application.Features.OS.ListarOS;
using TechChallenge.Application.Features.OS.ListarOSOficina;
using TechChallenge.Application.Features.OS.ObterOS;
using TechChallenge.Application.Features.OS.RegistrarDiagnostico;
using TechChallenge.Application.Features.Clientes.AtualizarCliente;
using TechChallenge.Application.Features.Clientes.CriarCliente;
using TechChallenge.Application.Features.Clientes.ExcluirCliente;
using TechChallenge.Application.Features.Clientes.ListarClientes;
using TechChallenge.Application.Features.Clientes.ObterCliente;
using TechChallenge.Application.Features.Funcionarios.AtualizarFuncionario;
using TechChallenge.Application.Features.Funcionarios.CriarFuncionario;
using TechChallenge.Application.Features.Funcionarios.ExcluirFuncionario;
using TechChallenge.Application.Features.Funcionarios.ListarFuncionarios;
using TechChallenge.Application.Features.Funcionarios.ObterFuncionario;
using TechChallenge.Application.Features.Inventario.AtualizarItemInventario;
using TechChallenge.Application.Features.Inventario.CriarItemInventario;
using TechChallenge.Application.Features.Inventario.ExcluirItemInventario;
using TechChallenge.Application.Features.Inventario.ListarInventario;
using TechChallenge.Application.Features.Inventario.ObterItemInventario;
using TechChallenge.Application.Features.Servicos.AtualizarServico;
using TechChallenge.Application.Features.Servicos.CriarServico;
using TechChallenge.Application.Features.Servicos.ExcluirServico;
using TechChallenge.Application.Features.Servicos.ListarServicos;
using TechChallenge.Application.Features.Servicos.ObterServico;
using TechChallenge.Application.Features.Veiculos.AtualizarVeiculo;
using TechChallenge.Application.Features.Veiculos.CriarVeiculo;
using TechChallenge.Application.Features.Veiculos.ExcluirVeiculo;
using TechChallenge.Application.Features.Veiculos.ListarVeiculos;
using TechChallenge.Application.Features.Veiculos.ObterVeiculo;
using TechChallenge.Application.Abstractions.Repositories;
using TechChallenge.Infrastructure.Database.Context;
using TechChallenge.Infrastructure.Database.Repositories;

namespace TechChallenge.Infrastructure.IoC.Injection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));
            });

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IEnderecoRepository, EnderecoRepository>();
            services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
            services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
            services.AddScoped<IOrdemServicoServicosRepository, OrdemServicoServicosRepository>();
            services.AddScoped<IOrdemServicoProdutosRepository, OrdemServicoProdutosRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IServicoRepository, ServicoRepository>();
            services.AddScoped<IVeiculoRepository, VeiculoRepository>();

            services.AddValidatorsFromAssemblyContaining<CriarClienteCommandValidator>();

            services.AddScoped<CriarOSService>();
            services.AddScoped<ObterOSService>();
            services.AddScoped<ListarOSService>();
            services.AddScoped<AtribuirOSService>();
            services.AddScoped<RegistrarDiagnosticoService>();
            services.AddScoped<ListarOSOficinaService>();
            services.AddScoped<ExcluirOSService>();

            services.AddScoped<CriarClienteService>();
            services.AddScoped<ObterClienteService>();
            services.AddScoped<ListarClientesService>();
            services.AddScoped<AtualizarClienteService>();
            services.AddScoped<ExcluirClienteService>();

            services.AddScoped<CriarFuncionarioService>();
            services.AddScoped<ObterFuncionarioService>();
            services.AddScoped<ListarFuncionariosService>();
            services.AddScoped<AtualizarFuncionarioService>();
            services.AddScoped<ExcluirFuncionarioService>();

            services.AddScoped<CriarItemInventarioService>();
            services.AddScoped<ObterItemInventarioService>();
            services.AddScoped<ListarInventarioService>();
            services.AddScoped<AtualizarItemInventarioService>();
            services.AddScoped<ExcluirItemInventarioService>();

            services.AddScoped<CriarServicoService>();
            services.AddScoped<ObterServicoService>();
            services.AddScoped<ListarServicosService>();
            services.AddScoped<AtualizarServicoService>();
            services.AddScoped<ExcluirServicoService>();

            services.AddScoped<CriarVeiculoService>();
            services.AddScoped<ObterVeiculoService>();
            services.AddScoped<ListarVeiculosService>();
            services.AddScoped<AtualizarVeiculoService>();
            services.AddScoped<ExcluirVeiculoService>();

            return services;
        }
    }
}
