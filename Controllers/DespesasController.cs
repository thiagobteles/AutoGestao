using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class DespesasController(ApplicationDbContext context) : StandardGridController<Despesa>(context)
    {
    }
}