using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Models.Grid;
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

        public static ReferenceMetadata GetReferenceMetadata<T>() where T : class
        {
            var metadata = new ReferenceMetadata();
            var properties = typeof(T).GetProperties();

            foreach (var prop in properties)
            {
                // ✅ NOVO: Verificar atributos dedicados de Reference
                var referenceTextAttr = prop.GetCustomAttribute<ReferenceTextAttribute>();
                var referenceSearchableAttr = prop.GetCustomAttribute<ReferenceSearchableAttribute>();
                var referenceSubtitleAttrs = prop.GetCustomAttributes<ReferenceSubtitleAttribute>().ToList();

                // TextField - verificar atributo dedicado primeiro
                if (referenceTextAttr != null)
                {
                    metadata.TextProperty = new ReferencePropertyInfo
                    {
                        Property = prop,
                        NavigationPath = referenceTextAttr.NavigationPath
                    };
                }

                // Searchable fields - verificar atributo dedicado primeiro
                if (referenceSearchableAttr != null)
                {
                    // ✅ IMPORTANTE: Buscar Format do ReferenceSubtitleAttribute do mesmo campo (se existir)
                    var subtitleAttr = referenceSubtitleAttrs.FirstOrDefault();

                    metadata.SearchableProperties.Add(new ReferencePropertyInfo
                    {
                        Property = prop,
                        NavigationPath = referenceSearchableAttr.NavigationPath,
                        Format = subtitleAttr?.Format // Copiar Format do subtitle para o searchable
                    });
                }

                // Subtitle fields - verificar atributos dedicados primeiro
                if (referenceSubtitleAttrs.Any())
                {
                    foreach (var subtitleAttr in referenceSubtitleAttrs)
                    {
                        metadata.SubtitleProperties.Add(new ReferencePropertyInfo
                        {
                            Property = prop,
                            NavigationPath = subtitleAttr.NavigationPath,
                            Prefix = subtitleAttr.Prefix,
                            Order = subtitleAttr.Order,
                            Format = subtitleAttr.Format
                        });
                    }
                }

                // ✅ FALLBACK: Verificar GridFieldAttribute (backward compatibility)
                var gridFieldAttr = prop.GetCustomAttribute<GridFieldAttribute>();
                if (gridFieldAttr != null)
                {
                    // TextField (IsText = true) - apenas se não foi definido via atributo dedicado
                    if (gridFieldAttr.IsText && metadata.TextProperty == null)
                    {
                        metadata.TextProperty = new ReferencePropertyInfo
                        {
                            Property = prop,
                            NavigationPath = gridFieldAttr.NavigationPath
                        };
                    }

                    // Searchable fields - apenas se não foi definido via atributo dedicado
                    if (gridFieldAttr.IsSearchable && !metadata.SearchableProperties.Any(p => p.Property == prop))
                    {
                        metadata.SearchableProperties.Add(new ReferencePropertyInfo
                        {
                            Property = prop,
                            NavigationPath = gridFieldAttr.NavigationPath
                        });
                    }

                    // Subtitle fields - apenas se não foi definido via atributo dedicado
                    if (gridFieldAttr.IsSubtitle && !metadata.SubtitleProperties.Any(p => p.Property == prop))
                    {
                        metadata.SubtitleProperties.Add(new ReferencePropertyInfo
                        {
                            Property = prop,
                            NavigationPath = gridFieldAttr.NavigationPath,
                            Prefix = gridFieldAttr.SubtitlePrefix,
                            Order = gridFieldAttr.SubtitleOrder,
                            Format = gridFieldAttr.Format
                        });
                    }
                }
            }

            // Ordenar subtitles
            metadata.SubtitleProperties = [.. metadata.SubtitleProperties.OrderBy(p => p.Order)];

            return metadata;
        }

        #region MÉTODOS PRIVADOS

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

            var compositeAttr = prop.GetCustomAttribute<GridCompositeAttribute>();
            if (compositeAttr != null)
            {
                return new GridColumn
                {
                    Name = prop.Name,
                    DisplayName = compositeAttr.DisplayName ?? prop.Name,
                    Width = compositeAttr.Width,
                    Sortable = false,
                    Type = EnumGridColumnType.Custom,
                    CustomRender = item => RenderCompositeField(item, compositeAttr)
                };
            }

            if (attr == null)
            {
                return null;
            }

            var column = new GridColumn
            {
                Name = prop.Name,
                DisplayName = attr.DisplayName ?? GetDisplayName(prop),
                Width = attr.Width,
                Sortable = attr.Sortable,
                Format = attr.Format
            };

            // Verificar se é campo de Imagem ou Arquivo via FormField
            var formFieldAttr = prop.GetCustomAttribute<FormFieldAttribute>();

            // NOVO: Detectar campos de imagem e arquivo
            if (formFieldAttr?.Type == EnumFieldType.Image || formFieldAttr?.Type == EnumFieldType.File)
            {
                column.Type = EnumGridColumnType.Text; // Será renderizado na view _GridCell.cshtml
                column.Sortable = false;
                column.Width = formFieldAttr.Type == EnumFieldType.Image
                    ? (attr.Width ?? "80px")
                    : (attr.Width ?? "150px");
            }
            // Determinar o tipo da coluna
            else if (prop.PropertyType.IsEnum || Nullable.GetUnderlyingType(prop.PropertyType)?.IsEnum == true)
            {
                column.Type = EnumGridColumnType.Enumerador;
                column.EnumRender = attr.EnumRender;
            }
            else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(bool?))
            {
                column.Type = EnumGridColumnType.Enumerador;
            }
            else if (IsNumericType(prop.PropertyType))
            {
                if (!string.IsNullOrEmpty(attr.Format) && attr.Format.ToUpper() == "C")
                {
                    column.Type = EnumGridColumnType.Currency;
                }
                else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(int?) ||
                         prop.PropertyType == typeof(long) || prop.PropertyType == typeof(long?))
                {
                    column.Type = EnumGridColumnType.Integer;
                }
                else
                {
                    column.Type = EnumGridColumnType.Number;
                }
            }
            else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
            {
                column.Type = EnumGridColumnType.Date;
            }
            else
            {
                column.Type = EnumGridColumnType.Text;
            }

            // Link para Details se for GridMain
            if (prop.GetCustomAttribute<GridMainAttribute>() != null)
            {
                column.UrlAction = "Details";
            }

            var docAttr = prop.GetCustomAttribute<GridDocumentAttribute>();
            if (docAttr != null)
            {
                column.Type = EnumGridColumnType.Custom;
                column.CustomRender = item => RenderDocumentField(item, prop, docAttr);
            }

            return column;
        }

        private static string RenderCompositeField(object item, GridCompositeAttribute attr)
        {
            var values = new List<string>();

            foreach (var navPath in attr.NavigationPaths)
            {
                var value = GetNestedPropertyValue(item, navPath);
                values.Add(value?.ToString() ?? "N/A");
            }

            return !string.IsNullOrEmpty(attr.Template) 
                ? string.Format(attr.Template, [.. values])
                : string.Join(attr.Separator, values);
        }

        private static string RenderDocumentField(object item, PropertyInfo property, GridDocumentAttribute attr)
        {
            var value = property.GetValue(item);
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return "<span class='text-muted'>—</span>";
            }

            var formatted = ApplyFormat(value.ToString()!, attr.Format ?? "");

            if (attr.DocumentType == DocumentType.CPF)
            {
                return $"<span class='badge bg-primary bg-opacity-10 text-primary'>{formatted}</span>";
            }
            else if (attr.DocumentType == DocumentType.CNPJ)
            {
                return $"<span class='badge bg-success bg-opacity-10 text-success'>{formatted}</span>";
            }

            return formatted;
        }

        private static object? GetNestedPropertyValue(object obj, string propertyPath)
        {
            if (obj == null)
            {
                return null;
            }

            foreach (var propName in propertyPath.Split('.'))
            {
                var propInfo = obj.GetType().GetProperty(propName);
                if (propInfo == null)
                {
                    return null;
                }

                obj = propInfo.GetValue(obj);
                if (obj == null)
                {
                    return null;
                }
            }

            return obj;
        }

        private static bool IsNumericType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type == typeof(int) || type == typeof(long) ||
                   type == typeof(short) || type == typeof(byte) ||
                   type == typeof(decimal) || type == typeof(double) ||
                   type == typeof(float);
        }

        private static string GetDisplayName(PropertyInfo property)
        {
            var display = property.GetCustomAttribute<DisplayAttribute>();
            if (display?.Name != null)
            {
                return display.Name;
            }

            var displayName = property.GetCustomAttribute<DisplayNameAttribute>();
            return displayName?.DisplayName != null 
                ? displayName.DisplayName
                : property.Name;
        }

        private static string ApplyFormat(string str, string format)
        {
            if (string.IsNullOrEmpty(format) || string.IsNullOrEmpty(str))
            {
                return str ?? "";
            }

            // Remove caracteres não numéricos
            str = new string([.. str.Where(char.IsDigit)]);
            if (string.IsNullOrEmpty(str))
            {
                return str ?? "";
            }

            return format switch
            {
                "###.###.###-##" when str.Length == 11
                    => $"{str[..3]}.{str.Substring(3, 3)}.{str.Substring(6, 3)}-{str.Substring(9, 2)}",

                "##.###.###/####-##" when str.Length == 14
                    => $"{str[..2]}.{str.Substring(2, 3)}.{str.Substring(5, 3)}/{str.Substring(8, 4)}-{str.Substring(12, 2)}",

                "(##) ####-####" when str.Length == 10
                    => $"({str[..2]}) {str.Substring(2, 4)}-{str.Substring(6, 4)}",

                "(##) #####-####" when str.Length == 11
                    => $"({str[..2]}) {str.Substring(2, 5)}-{str.Substring(7, 4)}",

                "#####-###" when str.Length == 8
                    => $"{str[..5]}-{str.Substring(5, 3)}",

                "C" => decimal.TryParse(str, out var currency)
                    ? currency.ToString("C2")
                    : str,

                _ => str
            };
        }

        #endregion MÉTODOS PRIVADOS
    }
}