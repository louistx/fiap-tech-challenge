namespace TechChallenge.Api.Models.Request;

public class CriarClienteRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Rg { get; set; } = string.Empty;
    public EnderecoRequest Endereco { get; set; } = new();
}

public class AtualizarClienteRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Rg { get; set; } = string.Empty;
    public EnderecoRequest Endereco { get; set; } = new();
}
