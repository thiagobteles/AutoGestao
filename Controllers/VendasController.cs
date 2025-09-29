using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class VendasController(ApplicationDbContext context) : StandardGridController<Venda>(context)
    {
    }
}