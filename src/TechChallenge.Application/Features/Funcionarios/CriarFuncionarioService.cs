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
        var funcionario = new Funcionario
        {
            Id = Guid.NewGuid(),
            Nome = command.Nome,
            CPF = command.CPF,
            RG = command.RG,
            Endereco = command.Endereco,
            Cargo = command.Cargo
        };

        _funcionarioRepository.AddAsync(funcionario).GetAwaiter().GetResult();
        return true;
    }
}
