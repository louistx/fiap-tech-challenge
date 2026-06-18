namespace TechChallenge.Api.Models.Request;

public class CriarFuncionarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Rg { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public EnderecoRequest Endereco { get; set; } = new();
}

public class AtualizarFuncionarioRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Rg { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public EnderecoRequest Endereco { get; set; } = new();
}
