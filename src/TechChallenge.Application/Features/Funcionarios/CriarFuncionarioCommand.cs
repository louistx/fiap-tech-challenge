namespace TechChallenge.Application.Features.Funcionarios;

public class CriarFuncionarioCommand
{
    public string Nome { get; set; }
    public string CPF { get; set; }
    public string RG { get; set; }
    public string Endereco { get; set; }
    public string Cargo { get; set; }     // Vendedor / Mecanico / Administrador
}