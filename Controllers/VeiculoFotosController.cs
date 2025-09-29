using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class VeiculoFotosController(ApplicationDbContext context) : StandardGridController<VeiculoFoto>(context)
    {
    }
}