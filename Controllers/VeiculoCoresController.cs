using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class VeiculoCoresController(ApplicationDbContext context) : StandardGridController<VeiculoCor>(context)
    {
    }
}