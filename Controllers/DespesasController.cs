using AutoGestao.Data;
using AutoGestao.Entidades;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class DespesasController(ApplicationDbContext context) : StandardGridController<Despesa>(context)
    {
        protected override IQueryable<Despesa> GetBaseQuery()
        {
            return _context.Despesas
                .Include(v => v.Veiculo)
                .Include(v => v.DespesaTipo)
                .Include(v => v.Fornecedor)
                .OrderByDescending(v => v.Id)
                .AsQueryable();
        }
    }
}