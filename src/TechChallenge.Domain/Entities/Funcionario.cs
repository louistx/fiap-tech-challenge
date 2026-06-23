using TechChallenge.Domain.Enums;

namespace TechChallenge.Domain.Entities;

public class Funcionario
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Rg { get; set; } = string.Empty;
    public eTipoFuncionario TipoFuncionario { get; set; }
    public Guid? EnderecoId { get; set; }
    public Endereco Endereco { get; set; } = null!;
}
