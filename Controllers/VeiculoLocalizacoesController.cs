using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class VeiculoLocalizacoesController(ApplicationDbContext context) : StandardGridController<VeiculoLocalizacao>(context)
    {
    }
}