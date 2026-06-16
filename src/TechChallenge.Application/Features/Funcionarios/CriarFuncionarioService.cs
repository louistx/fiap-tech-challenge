namespace TechChallenge.Application.Features.Funcionarios;

public class CriarFuncionarioService
{
    private readonly IFuncionarioRepository _funcionarioRepository;

    public CriarFuncionarioService(IFuncionarioRepository funcionarioRepository)
    {
        _funcionarioRepository = funcionarioRepository;
    }

    public bool CriarFuncionario(CriarFuncionarioCommand command)
    {
        
    }
}