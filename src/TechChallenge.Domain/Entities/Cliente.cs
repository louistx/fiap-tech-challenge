namespace TechChallenge.Domain.Entities;

public class Cliente
{
    public Guid Id { get; set; }
    public string Nome { get; set; }
    public string Cpf { get; set; }
    public string Rg { get; set; }
    public Guid EnderecoId { get; set; }
    public Endereco Endereco { get; set; }
}