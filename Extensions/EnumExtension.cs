using FGT.Atributes;
using FGT.Enumerador;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FGT.Extensions
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
            if (field == null)
            {
                return source.ToString() ?? string.Empty;
            }

            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Description : source.ToString();
        }

        public static string GetCategory<T>(this T source)
        {
            var field = source.GetType().GetField(source.ToString());
            if (field == null)
            {
                return source.ToString() ?? string.Empty;
            }

            var attributes = (CategoryAttribute[])field.GetCustomAttributes(typeof(CategoryAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Category : source.ToString();
        }

        public static string GetIcone<T>(this T source)
        {
            var field = source.GetType().GetField(source.ToString());
            if (field == null)
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
            if (field == null)
            {
                return source.ToString() ?? string.Empty;
            }

            return field.GetValue(source).ToString();
        }

        public static List<SelectListItem> GetSelectListItems<TEnum>(bool obterIcone = true) where TEnum : struct, Enum
        {
            var options = new List<SelectListItem>();
            var enumDictionary = GetEnumDictionary<TEnum>();

            foreach (var item in enumDictionary.Where(x => x.Key != 0))
            {
                var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), item.Key);
                var descricao = enumValue.GetDescription();
                var icone = enumValue.GetIcone();

                // Se tiver ícone e obterIcone for true, adiciona um marcador especial
                // que será processado pelo JavaScript para criar custom selects
                var text = descricao.Trim();
                if (obterIcone && !string.IsNullOrEmpty(icone))
                {
                    // Formato: "[ICON:fas fa-check]Descrição"
                    text = $"[ICON:{icone}]{descricao.Trim()}";
                }

                options.Add(new SelectListItem
                {
                    Value = item.Key.ToString(),
                    Text = text
                });
            }

            return options;
        }

        /// <summary>
        /// Obtém opções de enum com informações de ícone para custom selects
        /// </summary>
        public static List<EnumOption> GetEnumOptions<TEnum>() where TEnum : struct, Enum
        {
            var options = new List<EnumOption>();
            var enumDictionary = GetEnumDictionary<TEnum>();

            foreach (var item in enumDictionary.Where(x => x.Key != 0))
            {
                var enumValue = (TEnum)Enum.ToObject(typeof(TEnum), item.Key);
                var descricao = enumValue.GetDescription();
                var icone = enumValue.GetIcone();

                options.Add(new EnumOption
                {
                    Value = item.Key.ToString(),
                    Text = descricao.Trim(),
                    Icon = icone
                });
            }

            return options;
        }
    }

    /// <summary>
    /// Classe para representar uma opção de enum com ícone
    /// </summary>
    public class EnumOption
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public string Icon { get; set; }
    }
}