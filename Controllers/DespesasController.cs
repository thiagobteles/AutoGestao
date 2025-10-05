using AutoGestao.Data;
using AutoGestao.Entidades;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class DespesasController(ApplicationDbContext context) : StandardGridController<Despesa>(context)
    {
    }
}