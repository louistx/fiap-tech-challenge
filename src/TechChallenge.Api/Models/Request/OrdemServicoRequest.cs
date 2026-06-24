using System;
namespace TechChallenge.Api.Models.Request;

public class CriarOrdemServicoRequest
{
    public string Descricao { get; set; } = string.Empty;
    public Guid ClienteResponsavelId { get; set; }
    public Guid FuncionarioResponsavelId { get; set; }
    public Guid VeiculoId { get; set; }
}

public class AtualizarOrdemServicoRequest
{
    public string Descricao { get; set; } = string.Empty;
    public Guid ClienteResponsavelId { get; set; }
    public Guid FuncionarioResponsavelId { get; set; }
    public Guid VeiculoId { get; set; }
}

public class AtribuirOrdemServicoRequest
{
    public Guid MecanicoId { get; set; }
}

public class RegistrarDiagnosticoRequest
{
    public List<Guid> ServicosIds { get; set; } = [];
    public List<Guid> ProdutosIds { get; set; } = [];
}
