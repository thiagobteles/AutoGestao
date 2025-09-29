using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class VeiculoMarcaModelosController(ApplicationDbContext context) : StandardGridController<VeiculoMarcaModelo>(context)
    {
    }
}