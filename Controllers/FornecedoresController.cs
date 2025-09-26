using AutoGestao.Data;
using AutoGestao.Entidades;

namespace AutoGestao.Controllers
{
    public class FornecedoresController(ApplicationDbContext context) : StandardGridController<Fornecedor>(context)
    {
    }
}