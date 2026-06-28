namespace TechChallenge.Api.Models.Request;

public class CriarClienteRequest
{
    public string? Nome { get; set; }
    public string? Cpf { get; set; }
    public string? Rg { get; set; }
    public EnderecoRequest? Endereco { get; set; }
}

public class AtualizarClienteRequest
{
    public string? Nome { get; set; }
    public string? Cpf { get; set; }
    public string? Rg { get; set; }
    public EnderecoRequest? Endereco { get; set; }
}
