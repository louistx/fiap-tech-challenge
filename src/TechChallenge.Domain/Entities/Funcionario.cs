namespace TechChallenge.Domain.Entities;

public class Funcionario
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string Rg { get; set; }
    public Endereco Endereco { get; set; }
    public enum TipoFuncionario
    {
        Administrador = 0,
        Vendedor,
        Mecanico,
    }
}