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
            if (field== null)
            {
                return null;
            }

            var attributes = (IconeAttribute[])field.GetCustomAttributes(typeof(IconeAttribute), false);

            // Retorna o ícone apenas se o atributo existir
            // Caso contrário retorna null em vez de source.ToString()
            return attributes != null && attributes.Length > 0 ? attributes[0].Icone : null;
        }

        public static string GetValueString<T>(this T source)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T não é do tipo Enum");
            }

            var field = source.GetType().GetField(source?.ToString());
            return field.GetValue(source).ToString();
        }

        public static List<SelectListItem> GetSelectListItems<TEnum>(bool obterIcone = false) where TEnum : struct, Enum
        {
            var options = new List<SelectListItem>();
            var enumDictionary = GetEnumDictionary<TEnum>();

            foreach (var item in enumDictionary.Where(x => x.Key != 0))
            {
                var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), item.Key);
                var icone = enumValue.GetIcone();
                var descricao = enumValue.GetDescription();

                if (obterIcone && !string.IsNullOrEmpty(icone))
                {
                    options.Add(new SelectListItem
                    {
                        Value = item.Key.ToString(),
                        Text = $"{icone} {descricao}".Trim()
                    });
                }
                else
                {
                    options.Add(new SelectListItem
                    {
                        Value = item.Key.ToString(),
                        Text = descricao.Trim()
                    });
                }
            }

            return options;
        }
    }
}