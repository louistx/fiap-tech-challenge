namespace TechChallenge.Application.Features.OS.RegistrarDiagnostico;

public class RegistrarDiagnosticoCommand
{
    public int OrdemServicoId { get; set; }
    public List<int> ServicosIds { get; set; }
    public List<int> ItensInventarioIds { get; set; }
}