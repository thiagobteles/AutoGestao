using Microsoft.AspNetCore.Mvc;
using FGT.Models;

namespace FGT.Views.Shared.Components.StandardGrid
{
    public class StandardGridViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(StandardGridViewModel model)
        {
            return View(model);
        }
    }
}