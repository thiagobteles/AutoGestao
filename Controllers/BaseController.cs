using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Models;
using AutoGestao.Extensions;
using AutoGestao.Enumerador;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AutoGestao.Controllers
{
    public class BaseController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public SelectList CreateEnumSelectList<T>(T? selectedValue = null) where T : struct, Enum
        {
            var enumDictionary = EnumExtension.GetEnumDictionary<T>();

            // Remover o valor "Nenhum" se existir (valor 0)
            if (enumDictionary.ContainsKey(0))
            {
                enumDictionary.Remove(0);
            }

            var selectList = enumDictionary.Select(kvp => new SelectListItem
            {
                Value = kvp.Key.ToString(),
                Text = kvp.Value,
                Selected = selectedValue != null && kvp.Key == Convert.ToInt32(selectedValue)
            }).ToList();

            return new SelectList(selectList, "Value", "Text", selectedValue?.ToString());
        }

        /// <summary>
        /// Action para obter dados de enum via AJAX (útil para carregamentos dinâmicos)
        /// </summary>
        [HttpGet]
        public IActionResult GetEnumData<T>()
        {
            try
            {
                object result = EnumExtension.GetEnumDictionary<T>()
                       .Where(kvp => kvp.Key != 0) // Remove "Nenhum"
                       .Select(kvp => new { value = kvp.Key, text = kvp.Value });

                return Json(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}