using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class TarefasController(ApplicationDbContext context) : StandardGridController<Tarefa>(context)
    {
    }
}