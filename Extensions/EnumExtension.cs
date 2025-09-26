using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Extensions
{
    public static class EnumExtension
    {
        public static Dictionary<int, string> GetEnumDictionary<T>()
        {
            return !typeof(T).IsEnum
                ? throw new ArgumentException("T não é do tipo Enum")
                : Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(t => (int)(object)t, t => t.GetDescription());
        }

        public static string GetDescription<T>(this T source)
        {
            var field = source.GetType().GetField(source?.ToString());
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Description : source.ToString();
        }

        public static string GetCategory<T>(this T source)
        {
            var field = source.GetType().GetField(source.ToString());
            var attributes = (CategoryAttribute[])field.GetCustomAttributes(typeof(CategoryAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Category : source.ToString();
        }

        public static string GetIcone<T>(this T source)
        {
            var field = source.GetType().GetField(source.ToString());
            var attributes = (IconeAttribute[])field.GetCustomAttributes(typeof(IconeAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Icone : source.ToString();
        }

        public static List<SelectListItem> GetSelectListItems<TEnum>(bool obterIcone = false) where TEnum : struct, Enum
        {
            var options = new List<SelectListItem>();
            var enumDictionary = GetEnumDictionary<TEnum>();

            foreach (var item in enumDictionary.Where(x => x.Key != 0))
            {
                var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), item.Key);

                if (obterIcone && !string.IsNullOrEmpty(enumValue.GetIcone()))
                {
                    options.Add(new SelectListItem
                    {
                        Value = item.Key.ToString(),
                        Text = $"{enumValue.GetIcone()} {enumValue.GetDescription()}".Trim()
                    });
                }
                else
                {
                    options.Add(new SelectListItem
                    {
                        Value = item.Key.ToString(),
                        Text = enumValue.GetDescription().Trim()
                    });
                }
                
            }
            return options;
        }
    }
}