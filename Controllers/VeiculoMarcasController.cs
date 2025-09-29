using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class VeiculoMarcasController(ApplicationDbContext context) : StandardGridController<VeiculoMarca>(context)
    {
    }
}