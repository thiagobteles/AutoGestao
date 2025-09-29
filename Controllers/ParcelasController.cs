using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class ParcelasController(ApplicationDbContext context) : StandardGridController<Parcela>(context)
    {
    }
}