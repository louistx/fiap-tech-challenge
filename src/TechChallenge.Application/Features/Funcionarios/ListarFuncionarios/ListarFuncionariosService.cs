using TechChallenge.Domain.Entities;
using TechChallenge.Application.Abstractions.Repositories;

namespace TechChallenge.Application.Features.Funcionarios.ListarFuncionarios;

public class ListarFuncionariosService
{
    private readonly IFuncionarioRepository _funcionarioRepository;

    public ListarFuncionariosService(IFuncionarioRepository funcionarioRepository)
    {
        _funcionarioRepository = funcionarioRepository;
    }

    public List<Funcionario> ListarFuncionarios(ListarFuncionariosQuery query)
    {
        return _funcionarioRepository.GetAllAsync().GetAwaiter().GetResult();
    }
}
