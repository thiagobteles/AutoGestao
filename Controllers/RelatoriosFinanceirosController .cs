using AutoGestao.Atributes;
using Microsoft.AspNetCore.Mvc;

namespace AutoGestao.Controllers
{
    [RequiredRoles("Admin", "Financeiro")]
    public class RelatoriosFinanceirosController : Controller
    {
        public RelatoriosFinanceirosController()
        {
        }
    }
}