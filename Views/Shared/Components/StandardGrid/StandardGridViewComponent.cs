using Microsoft.AspNetCore.Mvc;
using AutoGestao.Models;

namespace AutoGestao.Views.Shared.Components.StandardGrid
{
    public class StandardGridViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(StandardGridViewModel model)
        {
            return View(model);
        }
    }
}