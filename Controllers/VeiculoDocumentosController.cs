using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;

namespace AutoGestao.Controllers
{
    public class VeiculoDocumentosController(ApplicationDbContext context) : StandardGridController<VeiculoDocumento>(context)
    {
    }
}