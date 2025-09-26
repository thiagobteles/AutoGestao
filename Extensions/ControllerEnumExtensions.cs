using AutoGestao.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AutoGestao.Extensions
{
    /// <summary>
    /// Extensions para Controllers usarem a automação de Enums
    /// </summary>
    public static class ControllerEnumExtensions
    {
        /// <summary>
        /// Popula automaticamente todos os Enums na ViewBag
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="controller">Controller</param>
        /// <param name="includeIcons">Se deve incluir ícones</param>
        public static void PopulateEnums<T>(this Controller controller, bool includeIcons = true)
        {
            EnumAutomationHelper.PopulateEnumsInViewBag<T>(controller.ViewBag, includeIcons);
        }

        /// <summary>
        /// Popula automaticamente todos os Enums no ViewData
        /// </summary>
        /// <typeparam name="T">Tipo da entidade</typeparam>
        /// <param name="controller">Controller</param>
        /// <param name="includeIcons">Se deve incluir ícones</param>
        public static void PopulateEnumsInViewData<T>(this Controller controller, bool includeIcons = true)
        {
            EnumAutomationHelper.PopulateEnumsInViewData<T>(controller.ViewData, includeIcons);
        }
    }
}
