using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class AvaliacoesController(ApplicationDbContext context) : StandardGridController<Avaliacao>(context)
    {
    }
}