using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class VeiculoFiliaisController(ApplicationDbContext context) : StandardGridController<VeiculoFilial>(context)
    {
    }
}