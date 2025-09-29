using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class EmpresaController(ApplicationDbContext context) : StandardGridController<Empresa>(context)
    {
    }
}