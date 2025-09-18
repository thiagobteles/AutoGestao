using Microsoft.AspNetCore.Mvc.Rendering;
using AutoGestao.Extensions;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Helpers
{
    /// <summary>
    /// Helper para criação de SelectLists baseadas em Enums usando GetDescription()
    /// </summary>
    public static class EnumSelectListHelper
    {
        /// <summary>
        /// Cria uma SelectList baseada em um Enum usando GetDescription()
        /// </summary>
        /// <typeparam name="T">Tipo do Enum</typeparam>
        /// <param name="selectedValue">Valor selecionado (opcional)</param>
        /// <param name="includeEmpty">Se deve incluir opção vazia</param>
        /// <param name="emptyText">Texto da opção vazia</param>
        /// <param name="excludeNoneValue">Se deve excluir o valor "Nenhum" (0)</param>
        /// <returns>SelectList configurada</returns>
        public static SelectList CreateSelectList<T>(
            T? selectedValue = null,
            bool includeEmpty = true,
            string emptyText = "Selecione uma opção...",
            bool excludeNoneValue = true) where T : struct, Enum
        {
            var enumDictionary = EnumExtension.GetEnumDictionary<T>();

            // Remover o valor "Nenhum" se solicitado (geralmente valor 0)
            if (excludeNoneValue && enumDictionary.ContainsKey(0))
            {
                enumDictionary.Remove(0);
            }

            var selectItems = enumDictionary.Select(kvp => new SelectListItem
            {
                Value = kvp.Key.ToString(),
                Text = kvp.Value,
                Selected = selectedValue != null && kvp.Key == Convert.ToInt32(selectedValue)
            }).ToList();

            // Adicionar opção vazia se solicitado
            if (includeEmpty)
            {
                selectItems.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = emptyText,
                    Selected = selectedValue == null
                });
            }

            return new SelectList(selectItems, "Value", "Text", selectedValue?.ToString());
        }

        /// <summary>
        /// Cria uma SelectList para filtros (inclui opção "Todos")
        /// </summary>
        /// <typeparam name="T">Tipo do Enum</typeparam>
        /// <param name="selectedValue">Valor selecionado (opcional)</param>
        /// <param name="allText">Texto da opção "Todos"</param>
        /// <returns>SelectList para filtros</returns>
        public static SelectList CreateFilterSelectList<T>(
            T? selectedValue = null,
            string allText = "Todos") where T : struct, Enum
        {
            var enumDictionary = EnumExtension.GetEnumDictionary<T>();

            // Remover o valor "Nenhum" se existir
            if (enumDictionary.ContainsKey(0))
            {
                enumDictionary.Remove(0);
            }

            var selectItems = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = allText,
                    Selected = selectedValue == null
                }
            };

            selectItems.AddRange(enumDictionary.Select(kvp => new SelectListItem
            {
                Value = kvp.Key.ToString(),
                Text = kvp.Value,
                Selected = selectedValue != null && kvp.Key == Convert.ToInt32(selectedValue)
            }));

            return new SelectList(selectItems, "Value", "Text", selectedValue?.ToString());
        }

        /// <summary>
        /// Obtém apenas as opções do enum sem criar SelectList (útil para JSON/AJAX)
        /// </summary>
        /// <typeparam name="T">Tipo do Enum</typeparam>
        /// <param name="excludeNoneValue">Se deve excluir o valor "Nenhum"</param>
        /// <returns>Lista de objetos anônimos com Value e Text</returns>
        public static object GetEnumOptions<T>(bool excludeNoneValue = true) where T : struct, Enum
        {
            var enumDictionary = EnumExtension.GetEnumDictionary<T>();

            if (excludeNoneValue && enumDictionary.ContainsKey(0))
            {
                enumDictionary.Remove(0);
            }

            return enumDictionary.Select(kvp => new
            {
                Value = kvp.Key,
                Text = kvp.Value
            }).ToList();
        }

        /// <summary>
        /// Cria SelectList com ícones (para uso com JavaScript)
        /// </summary>
        /// <typeparam name="T">Tipo do Enum</typeparam>
        /// <param name="iconMapping">Dicionário mapeando valores do enum para ícones</param>
        /// <param name="selectedValue">Valor selecionado</param>
        /// <returns>Lista com texto e ícones</returns>
        public static object CreateSelectListWithIcons<T>(
            Dictionary<T, string> iconMapping,
            T? selectedValue = null) where T : struct, Enum
        {
            var enumDictionary = EnumExtension.GetEnumDictionary<T>();

            if (enumDictionary.ContainsKey(0))
            {
                enumDictionary.Remove(0);
            }

            var result = enumDictionary.Select(kvp =>
            {
                var enumValue = (T)Enum.ToObject(typeof(T), kvp.Key);
                var icon = iconMapping.ContainsKey(enumValue) ? iconMapping[enumValue] : "fas fa-circle";

                return new
                {
                    Value = kvp.Key,
                    Text = kvp.Value,
                    Icon = icon,
                    Selected = selectedValue != null && kvp.Key == Convert.ToInt32(selectedValue)
                };
            }).ToList();

            return result;
        }
    }

    /// <summary>
    /// Extensões para usar em Views
    /// </summary>
    public static class EnumHtmlHelperExtensions
    {
        /// <summary>
        /// Extensão para criar DropDownList baseado em Enum diretamente na View
        /// </summary>
        public static SelectList EnumSelectList<T>(
            this IHtmlHelper htmlHelper,
            T? selectedValue = null,
            bool includeEmpty = true,
            string emptyText = "Selecione uma opção...",
            bool excludeNoneValue = true) where T : struct, Enum
        {
            return EnumSelectListHelper.CreateSelectList<T>(selectedValue, includeEmpty, emptyText, excludeNoneValue);
        }

        /// <summary>
        /// Extensão para criar DropDownList de filtro baseado em Enum
        /// </summary>
        public static SelectList EnumFilterSelectList<T>(
            this IHtmlHelper htmlHelper,
            T? selectedValue = null,
            string allText = "Todos") where T : struct, Enum
        {
            return EnumSelectListHelper.CreateFilterSelectList<T>(selectedValue, allText);
        }
    }

    /// <summary>
    /// Configurações específicas para diferentes tipos de enum
    /// </summary>
    public static class EnumConfigurations
    {
        /// <summary>
        /// Configuração para TipoPessoa com ícones
        /// </summary>
        public static object GetTipoPessoaWithIcons<T>(T? selectedValue = null) where T : struct, Enum
        {
            var iconMapping = new Dictionary<object, string>
            {
                { 1, "fas fa-user" },      // Pessoa Física
                { 2, "fas fa-building" }   // Pessoa Jurídica
            };

            var enumDictionary = EnumExtension.GetEnumDictionary<T>();

            if (enumDictionary.ContainsKey(0))
            {
                enumDictionary.Remove(0);
            }

            return enumDictionary.Select(kvp => new
            {
                Value = kvp.Key,
                Text = kvp.Value,
                Icon = iconMapping.ContainsKey(kvp.Key) ? iconMapping[kvp.Key] : "fas fa-circle",
                Selected = selectedValue != null && kvp.Key == Convert.ToInt32(selectedValue)
            }).ToList();
        }

        /// <summary>
        /// Lista de estados brasileiros
        /// </summary>
        public static SelectList GetEstadosBrasil(string? selectedValue = null)
        {
            var estados = new List<SelectListItem>
            {
                new() { Value = "", Text = "Selecione o estado" },
                new() { Value = "AC", Text = "Acre" },
                new() { Value = "AL", Text = "Alagoas" },
                new() { Value = "AP", Text = "Amapá" },
                new() { Value = "AM", Text = "Amazonas" },
                new() { Value = "BA", Text = "Bahia" },
                new() { Value = "CE", Text = "Ceará" },
                new() { Value = "DF", Text = "Distrito Federal" },
                new() { Value = "ES", Text = "Espírito Santo" },
                new() { Value = "GO", Text = "Goiás" },
                new() { Value = "MA", Text = "Maranhão" },
                new() { Value = "MT", Text = "Mato Grosso" },
                new() { Value = "MS", Text = "Mato Grosso do Sul" },
                new() { Value = "MG", Text = "Minas Gerais" },
                new() { Value = "PA", Text = "Pará" },
                new() { Value = "PB", Text = "Paraíba" },
                new() { Value = "PR", Text = "Paraná" },
                new() { Value = "PE", Text = "Pernambuco" },
                new() { Value = "PI", Text = "Piauí" },
                new() { Value = "RJ", Text = "Rio de Janeiro" },
                new() { Value = "RN", Text = "Rio Grande do Norte" },
                new() { Value = "RS", Text = "Rio Grande do Sul" },
                new() { Value = "RO", Text = "Rondônia" },
                new() { Value = "RR", Text = "Roraima" },
                new() { Value = "SC", Text = "Santa Catarina" },
                new() { Value = "SP", Text = "São Paulo" },
                new() { Value = "SE", Text = "Sergipe" },
                new() { Value = "TO", Text = "Tocantins" }
            };

            return new SelectList(estados, "Value", "Text", selectedValue);
        }

        /// <summary>
        /// Configuração para diferentes tipos de documento com ícones
        /// </summary>
        public static object GetTipoDocumentoWithIcons<T>(T? selectedValue = null) where T : struct, Enum
        {
            var iconMapping = new Dictionary<object, string>
            {
                { 1, "fas fa-file-alt" },     // CRV
                { 2, "fas fa-id-card" },      // CRLV
                { 3, "fas fa-receipt" }       // Nota Fiscal
            };

            var enumDictionary = EnumExtension.GetEnumDictionary<T>();

            if (enumDictionary.ContainsKey(0))
            {
                enumDictionary.Remove(0);
            }

            return enumDictionary.Select(kvp => new
            {
                Value = kvp.Key,
                Text = kvp.Value,
                Icon = iconMapping.ContainsKey(kvp.Key) ? iconMapping[kvp.Key] : "fas fa-file",
                Selected = selectedValue != null && kvp.Key == Convert.ToInt32(selectedValue)
            }).ToList();
        }

        /// <summary>
        /// Configuração para situações de veículo com ícones
        /// </summary>
        public static object GetSituacaoVeiculoWithIcons<T>(T? selectedValue = null) where T : struct, Enum
        {
            var iconMapping = new Dictionary<object, string>
            {
                { 1, "fas fa-warehouse" },    // Estoque
                { 2, "fas fa-check-circle" }, // Vendido
                { 3, "fas fa-lock" },         // Reservado
                { 4, "fas fa-wrench" }        // Manutenção
            };

            var enumDictionary = EnumExtension.GetEnumDictionary<T>();

            if (enumDictionary.ContainsKey(0))
            {
                enumDictionary.Remove(0);
            }

            return enumDictionary.Select(kvp => new
            {
                Value = kvp.Key,
                Text = kvp.Value,
                Icon = iconMapping.ContainsKey(kvp.Key) ? iconMapping[kvp.Key] : "fas fa-circle",
                Selected = selectedValue != null && kvp.Key == Convert.ToInt32(selectedValue)
            }).ToList();
        }
    }

    /// <summary>
    /// Atributo de validação customizado para enums
    /// </summary>
    public class ValidacaoEnumAttribute(Type enumType, bool allowNone = false) : ValidationAttribute
    {
        private readonly Type _enumType = enumType;
        private readonly bool _allowNone = allowNone;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !_enumType.IsEnum)
            {
                return new ValidationResult("Valor inválido");
            }

            var enumValue = (int)value;

            if (!_allowNone && enumValue == 0)
            {
                return new ValidationResult("Selecione uma opção válida");
            }

            if (!Enum.IsDefined(_enumType, enumValue))
            {
                return new ValidationResult("Valor não válido para este campo");
            }

            return ValidationResult.Success;
        }
    }
}