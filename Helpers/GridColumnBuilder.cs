using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AutoGestao.Helpers
{
    /// <summary>
    /// Helper inteligente para gerar GridColumns e processar ReferenceItems
    /// A complexidade fica aqui, as anotações ficam simples
    /// </summary>
    public static class GridColumnBuilder
    {
        /// <summary>
        /// Gera colunas automaticamente usando convenções inteligentes
        /// </summary>
        public static List<GridColumn> BuildColumns<T>() where T : class
        {
            var columns = new List<GridColumn>();
            var properties = typeof(T).GetProperties()
                .Select(p => new PropertyContext(p))
                .Where(p => ShouldIncludeProperty(p))
                .OrderBy(p => GetEffectiveOrder(p))
                .ToList();

            // Adicionar coluna de ID sempre primeiro (se não tiver [GridId] explícito)
            if (!properties.Any(p => p.Property.Name == "Id" && p.HasGridField))
            {
                columns.Add(new GridColumn
                {
                    Name = "Id",
                    DisplayName = "Cód",
                    Type = EnumGridColumnType.Number,
                    Sortable = true,
                    Width = "65px"
                });
            }

            foreach (var propContext in properties)
            {
                var column = BuildColumnFromProperty(propContext);
                if (column != null)
                {
                    columns.Add(column);
                }
            }

            // Adicionar coluna de ações sempre por último
            columns.Add(new GridColumn
            {
                Name = "Actions",
                DisplayName = "Ações",
                Type = EnumGridColumnType.Actions,
                Sortable = false,
                Width = "100px"
            });

            return columns;
        }

        /// <summary>
        /// Context class para simplificar lógica de detecção
        /// </summary>
        private class PropertyContext(PropertyInfo property)
        {
            public PropertyInfo Property { get; } = property;
            public GridFieldAttribute? GridField { get; } = property.GetCustomAttribute<GridFieldAttribute>();
            public bool HasGridField => GridField != null;
        }

        private static bool ShouldIncludeProperty(PropertyContext context)
        {
            var prop = context.Property;

            // Ignorar propriedades do sistema
            if (IsSystemProperty(prop.Name))
            {
                return false;
            }

            // Ignorar coleções de navegação
            if (IsNavigationCollection(prop))
            {
                return false;
            }

            // Se tem [GridField] com ShowInGrid = false, não incluir
            if (context.HasGridField && !context.GridField!.ShowInGrid)
            {
                return false;
            }

            // Incluir se tem [GridField] explícito
            if (context.HasGridField)
            {
                return true;
            }

            // Incluir propriedades comuns por convenção
            return IsCommonProperty(prop.Name);
        }

        private static bool IsSystemProperty(string name)
        {
            return name == "DataCadastro" ||
                   name == "DataAlteracao" ||
                   name == "UsuarioCadastro" ||
                   name == "UsuarioAlteracao";
        }

        private static bool IsNavigationCollection(PropertyInfo property)
        {
            return typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) &&
                   property.PropertyType != typeof(string);
        }

        private static bool IsCommonProperty(string name)
        {
            var commonProps = new[] {
                "Nome", "Descricao", "Email", "Telefone", "Celular", "CPF", "CNPJ",
                "Ativo", "Status", "Cidade", "Estado", "Endereco", "Codigo",
                "Placa", "Chassi", "Valor", "Preco", "Quantidade", "Estoque"
            };

            return commonProps.Contains(name);
        }

        private static int GetEffectiveOrder(PropertyContext context)
        {
            // 1. Se tem [GridField] com Order definido, usar
            if (context.HasGridField && context.GridField!.Order >= 0)
            {
                return context.GridField.Order;
            }

            // 2. Tentar [Display(Order)]
            var display = context.Property.GetCustomAttribute<DisplayAttribute>();
            if (display?.Order != null)
            {
                return display.Order;
            }

            // 3. Ordem por convenção baseada no nome
            return GetConventionalOrder(context.Property.Name);
        }

        private static int GetConventionalOrder(string propertyName)
        {
            return propertyName.ToLower() switch
            {
                "id" => 0,
                "codigo" => 5,
                "nome" => 10,
                "descricao" => 15,
                "razaosocial" => 10,
                "tipo" => 20,
                "cpf" => 30,
                "cnpj" => 30,
                "rg" => 35,
                "email" => 50,
                "telefone" => 60,
                "celular" => 65,
                "endereco" => 70,
                "cidade" => 75,
                "estado" => 80,
                "cep" => 82,
                "placa" => 10,
                "chassi" => 20,
                "valor" => 85,
                "preco" => 85,
                "quantidade" => 87,
                "estoque" => 87,
                "ativo" => 999,
                "status" => 999,
                _ => 100
            };
        }

        private static GridColumn? BuildColumnFromProperty(PropertyContext context)
        {
            var prop = context.Property;
            var attr = context.GridField;

            var column = new GridColumn
            {
                Name = prop.Name,
                DisplayName = GetDisplayName(context),
                Sortable = attr?.Sortable ?? true,
                Width = attr?.Width ?? GetDefaultWidth(prop),
                Type = DetermineColumnType(prop)
            };

            // Se IsLink = true, criar link para Details
            if (attr?.IsLink == true)
            {
                column.UrlAction = "Details";
            }

            // Se tem formato, criar CustomRender
            if (!string.IsNullOrEmpty(attr?.Format))
            {
                column.Format = attr.Format;
                column.Type = EnumGridColumnType.Custom;
                column.CustomRender = (obj) => FormatValue(obj, prop, attr.Format);
            }

            return column;
        }

        private static string GetDisplayName(PropertyContext context)
        {
            var prop = context.Property;
            var attr = context.GridField;

            // 1. [GridField(DisplayName)]
            if (!string.IsNullOrEmpty(attr?.DisplayName))
            {
                return attr.DisplayName;
            }

            // 2. [Display(Name)]
            var display = prop.GetCustomAttribute<DisplayAttribute>();
            if (!string.IsNullOrEmpty(display?.Name))
            {
                return display.Name;
            }

            // 3. [DisplayName]
            var displayName = prop.GetCustomAttribute<DisplayNameAttribute>();
            if (!string.IsNullOrEmpty(displayName?.DisplayName))
            {
                return displayName.DisplayName;
            }

            // 4. Gerar nome amigável
            return GenerateFriendlyName(prop.Name);
        }

        private static string GenerateFriendlyName(string propertyName)
        {
            // Casos especiais comuns
            return propertyName switch
            {
                "CPF" => "CPF",
                "CNPJ" => "CNPJ",
                "RG" => "RG",
                "CEP" => "CEP",
                _ => System.Text.RegularExpressions.Regex.Replace(propertyName, "([a-z])([A-Z])", "$1 $2")
            };
        }

        private static string? GetDefaultWidth(PropertyInfo property)
        {
            var propType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            // Width por tipo
            if (propType == typeof(bool))
            {
                return "65px";
            }

            if (propType == typeof(int) || propType == typeof(long))
            {
                return "80px";
            }

            if (propType == typeof(DateTime) || propType == typeof(DateOnly))
            {
                return "110px";
            }

            // Width por nome da propriedade
            return property.Name.ToLower() switch
            {
                "id" => "65px",
                "cpf" => "130px",
                "cnpj" => "160px",
                "cep" => "100px",
                "estado" or "uf" => "65px",
                "ativo" or "status" => "65px",
                _ => null // Auto
            };
        }

        private static EnumGridColumnType DetermineColumnType(PropertyInfo property)
        {
            var propType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

            if (propType == typeof(bool))
            {
                return EnumGridColumnType.Boolean;
            }

            if (propType == typeof(DateTime) || propType == typeof(DateOnly))
            {
                return EnumGridColumnType.Date;
            }

            if (propType == typeof(decimal) || propType == typeof(double) || propType == typeof(float))
            {
                // Detectar Currency por nome
                var name = property.Name.ToLower();
                if (name.Contains("valor") || name.Contains("preco") || name.Contains("total") ||
                    name.Contains("custo") || name.Contains("desconto"))
                {
                    return EnumGridColumnType.Currency;
                }

                return EnumGridColumnType.Number;
            }

            if (propType == typeof(int) || propType == typeof(long) || propType == typeof(short))
            {
                return EnumGridColumnType.Integer;
            }

            if (propType.IsEnum)
            {
                return EnumGridColumnType.Enumerador;
            }

            return EnumGridColumnType.Text;
        }

        private static string FormatValue(object obj, PropertyInfo property, string format)
        {
            var value = property.GetValue(obj);
            if (value == null)
            {
                return "-";
            }

            var str = value.ToString() ?? "";

            return format switch
            {
                "###.###.###-##" when str.Length == 11
                    => $"{str.Substring(0, 3)}.{str.Substring(3, 3)}.{str.Substring(6, 3)}-{str.Substring(9, 2)}",

                "##.###.###/####-##" when str.Length == 14
                    => $"{str.Substring(0, 2)}.{str.Substring(2, 3)}.{str.Substring(5, 3)}/{str.Substring(8, 4)}-{str.Substring(12, 2)}",

                "(##) ####-####" when str.Length == 10
                    => $"({str.Substring(0, 2)}) {str.Substring(2, 4)}-{str.Substring(6, 4)}",

                "(##) #####-####" when str.Length == 11
                    => $"({str.Substring(0, 2)}) {str.Substring(2, 5)}-{str.Substring(7, 4)}",

                "#####-###" when str.Length == 8
                    => $"{str.Substring(0, 5)}-{str.Substring(5, 3)}",

                _ => str
            };
        }

        /// <summary>
        /// Extrai metadados para ReferenceItem de uma entidade
        /// </summary>
        public static ReferenceMetadata GetReferenceMetadata<T>() where T : class
        {
            var metadata = new ReferenceMetadata();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                var attr = prop.GetCustomAttribute<GridFieldAttribute>();
                if (attr == null)
                {
                    continue;
                }

                // TextField (IsText = true)
                if (attr.IsText)
                {
                    metadata.TextProperty = new ReferencePropertyInfo
                    {
                        Property = prop,
                        NavigationPath = attr.NavigationPath
                    };
                }

                // Searchable fields
                if (attr.IsSearchable)
                {
                    metadata.SearchableProperties.Add(new ReferencePropertyInfo
                    {
                        Property = prop,
                        NavigationPath = attr.NavigationPath
                    });
                }

                // Subtitle fields
                if (attr.IsSubtitle)
                {
                    metadata.SubtitleProperties.Add(new ReferencePropertyInfo
                    {
                        Property = prop,
                        NavigationPath = attr.NavigationPath,
                        Prefix = attr.SubtitlePrefix,
                        Order = attr.SubtitleOrder,
                        Format = attr.Format
                    });
                }
            }

            // Ordenar subtitles
            metadata.SubtitleProperties = metadata.SubtitleProperties
                .OrderBy(p => p.Order)
                .ToList();

            return metadata;
        }
    }

    #region Metadata Classes para ReferenceItem

    public class ReferenceMetadata
    {
        public ReferencePropertyInfo? TextProperty { get; set; }
        public List<ReferencePropertyInfo> SubtitleProperties { get; set; } = new();
        public List<ReferencePropertyInfo> SearchableProperties { get; set; } = new();
    }

    public class ReferencePropertyInfo
    {
        public PropertyInfo Property { get; set; } = null!;
        public string? NavigationPath { get; set; }
        public string? Prefix { get; set; }
        public int Order { get; set; }
        public string? Format { get; set; }
    }

    #endregion
}