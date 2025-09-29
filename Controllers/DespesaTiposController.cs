using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class DespesaTiposController(ApplicationDbContext context) : StandardGridController<DespesaTipo>(context)
    {
    }
}