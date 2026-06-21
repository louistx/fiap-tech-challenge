namespace TechChallenge.Application.Features.Inventario;

public class CriarItemInventarioService
{
    private readonly IInventarioRepository _inventarioRepository;

    public CriarItemInventarioService(IInventarioRepository inventarioRepository)
    {
        _inventarioRepository = inventarioRepository;
    }

    public bool CriarItemInventario(CriarItemInventarioCommand command)
    {

    }
}