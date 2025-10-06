using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Services;
using System.Reflection;

namespace AutoGestao.Helpers
{
    public static class GridRenderHelper
    {
        public static string RenderGridCell<T>(
            T entity,
            PropertyContext context,
            string? value,
            IFileStorageService? fileStorageService = null,
            long idEmpresa = 0)
        {
            if (context.GridField == null)
            {
                return value ?? "";
            }

            var property = context.Property;
            var formFieldAttr = property.GetCustomAttribute<FormFieldAttribute>();

            // RENDERIZAR IMAGEM NA GRID
            if (formFieldAttr?.Type == EnumFieldType.Image && !string.IsNullOrEmpty(value))
            {
                var size = formFieldAttr.ImageSize.Split('x');
                var width = size.Length > 0 ? size[0] : "50";
                var height = size.Length > 1 ? size[1] : "50";

                // Se tiver fileStorageService, gerar URL do MinIO
                if (fileStorageService != null && idEmpresa > 0)
                {
                    try
                    {
                        var entityName = typeof(T).Name;
                        var url = fileStorageService.GetDownloadUrlAsync(value, entityName, idEmpresa)
                            .GetAwaiter()
                            .GetResult();

                        return $@"<img src='{url}' 
                                     alt='Imagem' 
                                     class='img-thumbnail' 
                                     style='max-width:{width}px; max-height:{height}px; object-fit: cover; border-radius: 4px;' 
                                     loading='lazy'>";
                    }
                    catch
                    {
                        return "<i class='fas fa-image text-muted'></i>";
                    }
                }

                return "<i class='fas fa-image text-muted'></i>";
            }

            // RENDERIZAR ARQUIVO NA GRID
            if (formFieldAttr?.Type == EnumFieldType.File && !string.IsNullOrEmpty(value))
            {
                var fileName = Path.GetFileName(value);
                var extension = Path.GetExtension(fileName).ToLower();

                var icon = extension switch
                {
                    ".pdf" => "fa-file-pdf text-danger",
                    ".doc" or ".docx" => "fa-file-word text-primary",
                    ".xls" or ".xlsx" => "fa-file-excel text-success",
                    ".jpg" or ".jpeg" or ".png" or ".gif" => "fa-file-image text-info",
                    ".zip" or ".rar" => "fa-file-archive text-warning",
                    _ => "fa-file text-secondary"
                };

                return $"<i class='fas {icon} me-1'></i> <span class='small'>{fileName}</span>";
            }

            // Renderizar Enums
            if (property.PropertyType.IsEnum)
            {
                return RenderEnumValue(value, property, context.GridField.EnumRender);
            }

            // Renderizar booleanos
            if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
            {
                if (bool.TryParse(value, out bool boolValue))
                {
                    return boolValue
                        ? "<i class='fas fa-check-circle text-success'></i>"
                        : "<i class='fas fa-times-circle text-danger'></i>";
                }
                return "";
            }

            // Renderizar datas
            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
            {
                if (DateTime.TryParse(value, out DateTime dateValue))
                {
                    return dateValue.ToString("dd/MM/yyyy");
                }
            }

            // Aplicar formatação se especificada
            if (!string.IsNullOrEmpty(context.GridField.Format))
            {
                return ApplyFormat(value, context.GridField.Format, property.PropertyType);
            }

            // Renderizar documentos (CPF/CNPJ)
            if (context.GridField is GridDocumentAttribute docAttr)
            {
                return FormatDocument(value, docAttr.DocumentType);
            }

            return value ?? "";
        }

        private static string RenderEnumValue(string? value, PropertyInfo property, EnumRenderType renderType)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            try
            {
                var enumValue = Enum.Parse(property.PropertyType, value);
                var enumInfo = EnumHelper.GetEnumInfo(enumValue);

                return renderType switch
                {
                    EnumRenderType.Icon => $"<i class='{enumInfo.Icon}' style='color:{enumInfo.Color}'></i>",
                    EnumRenderType.IconDescription => $"<i class='{enumInfo.Icon}' style='color:{enumInfo.Color}'></i> {enumInfo.Description}",
                    EnumRenderType.Description => enumInfo.Description,
                    _ => enumInfo.Description
                };
            }
            catch
            {
                return value;
            }
        }

        private static string ApplyFormat(string? value, string format, Type propertyType)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            // Formato de moeda
            if (format == "C" && decimal.TryParse(value, out decimal decimalValue))
            {
                return decimalValue.ToString("C2");
            }

            // Formato de máscara (CPF, CNPJ, etc)
            if (format.Contains("#"))
            {
                return ApplyMask(value, format);
            }

            return value;
        }

        private static string ApplyMask(string value, string mask)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            var cleanValue = new string(value.Where(char.IsDigit).ToArray());
            var result = "";
            var valueIndex = 0;

            foreach (var maskChar in mask)
            {
                if (maskChar == '#')
                {
                    if (valueIndex < cleanValue.Length)
                    {
                        result += cleanValue[valueIndex];
                        valueIndex++;
                    }
                }
                else
                {
                    result += maskChar;
                }
            }

            return result;
        }

        private static string FormatDocument(string? value, DocumentType type)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }

            return type switch
            {
                DocumentType.CPF => ApplyMask(value, "###.###.###-##"),
                DocumentType.CNPJ => ApplyMask(value, "##.###.###/####-##"),
                _ => value
            };
        }
    }
}