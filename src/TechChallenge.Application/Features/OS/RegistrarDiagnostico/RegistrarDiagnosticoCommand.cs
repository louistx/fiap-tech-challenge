namespace TechChallenge.Application.Features.OS.RegistrarDiagnostico;

public class RegistrarDiagnosticoCommand
{
    public Guid OrdemServicoId { get; set; }
    public List<Guid> ServicosIds { get; set; } = [];
    public List<Guid> ProdutosIds { get; set; } = [];
}