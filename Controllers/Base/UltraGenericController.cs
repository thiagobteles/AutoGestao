using AutoGestao.Atributes;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AutoGestao.Controllers.Base
{
    /// <summary>
    /// Controller Ultra-Genérico Completo
    /// Combina AutoGridController + AutoFormController + Funcionalidades Avançadas
    /// 
    /// PARA USAR:
    /// 1. Herde desta classe: public class MinhaController : UltraGenericController<MinhaEntidade>
    /// 2. Configure os atributos na entidade
    /// 3. Pronto! Grid + Form + CRUD + Validações + Upload + Tabs funcionando automaticamente
    /// </summary>
    /// <typeparam name="T">Entidade que herda de BaseEntidade</typeparam>
    public abstract class UltraGenericController<T>(
        ApplicationDbContext context,
        IFileStorageService fileStorageService,
        ILogger<AutoGridController<T>>? logger = null) : AutoFormController<T>(context, fileStorageService, logger) where T : BaseEntidade, new()
    {

        #region Advanced Features

        /// <summary>
        /// API: Busca automática para referencias (AJAX)
        /// GET: /Controller/SearchReference?term=xxx&property=PropertyName
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> SearchReference(string term, string property, int page = 1, int pageSize = 10)
        {
            try
            {
                var propertyInfo = _metadata.GetPropertyByName(property);
                if (propertyInfo == null)
                {
                    return Json(new { success = false, message = "Propriedade não encontrada" });
                }

                var formField = propertyInfo.GetCustomAttribute<FormFieldAttribute>();
                if (formField?.Reference == null)
                {
                    return Json(new { success = false, message = "Propriedade não é uma referência" });
                }

                var results = await SearchReferenceEntitiesAsync(formField.Reference, term, page, pageSize);
                return Json(new { success = true, data = results });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao buscar referência para {Property}", property);
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API: Upload de arquivo automático (AJAX)
        /// POST: /Controller/UploadFile
        /// </summary>
        [HttpPost]
        public virtual async Task<IActionResult> UploadFile(IFormFile file, string propertyName)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new { success = false, message = "Arquivo não encontrado" });
                }

                var property = _metadata.GetPropertyByName(propertyName);
                if (property == null)
                {
                    return Json(new { success = false, message = "Propriedade não encontrada" });
                }

                var formField = property.GetCustomAttribute<FormFieldAttribute>();
                if (formField?.Type is not (EnumFieldType.File or EnumFieldType.Image))
                {
                    return Json(new { success = false, message = "Propriedade não é de arquivo" });
                }

                var filePath = await _fileStorageService.SaveFileAsync(
                    file,
                    typeof(T).Name,
                    GetCurrentEmpresaId());

                var fileUrl = await _fileStorageService.GetDownloadUrlAsync(
                    filePath,
                    typeof(T).Name,
                    GetCurrentEmpresaId());

                return Json(new
                {
                    success = true,
                    filePath = filePath,
                    fileName = file.FileName,
                    fileUrl = fileUrl,
                    message = "Arquivo enviado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao fazer upload do arquivo");
                return Json(new { success = false, message = $"Erro ao enviar arquivo: {ex.Message}" });
            }
        }

        /// <summary>
        /// API: Deletar arquivo (AJAX)
        /// POST: /Controller/DeleteFile
        /// </summary>
        [HttpPost]
        public virtual async Task<IActionResult> DeleteFile(string propertyName, string filePath)
        {
            try
            {
                await _fileStorageService.DeleteFileAsync(filePath, typeof(T).Name, GetCurrentEmpresaId());
                return Json(new { success = true, message = "Arquivo removido com sucesso!" });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao deletar arquivo {FilePath}", filePath);
                return Json(new { success = false, message = $"Erro ao remover arquivo: {ex.Message}" });
            }
        }

        /// <summary>
        /// API: Validação dinâmica de campo (AJAX)
        /// POST: /Controller/ValidateField
        /// </summary>
        [HttpPost]
        public virtual async Task<IActionResult> ValidateField(string propertyName, string value, [FromBody] T entity)
        {
            try
            {
                var property = _metadata.GetPropertyByName(propertyName);
                if (property == null)
                {
                    return Json(new { valid = false, message = "Propriedade não encontrada" });
                }

                // Aplicar validações específicas
                var validationResult = await ValidatePropertyAsync(property, value, entity);

                return Json(new
                {
                    valid = validationResult.IsValid,
                    message = validationResult.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao validar campo {Property}", propertyName);
                return Json(new { valid = false, message = "Erro interno de validação" });
            }
        }

        /// <summary>
        /// API: Obter opções dinâmicas para selects dependentes (AJAX)
        /// GET: /Controller/GetDependentOptions?parentProperty=xxx&parentValue=yyy&targetProperty=zzz
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> GetDependentOptions(string parentProperty, string parentValue, string targetProperty)
        {
            try
            {
                var options = await GetDependentSelectOptionsAsync(parentProperty, parentValue, targetProperty);
                return Json(new { success = true, options = options });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao obter opções dependentes");
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API: Export automático para Excel (AJAX)
        /// GET: /Controller/ExportExcel?filters=...
        /// </summary>
        [HttpGet]
        public virtual async Task<IActionResult> ExportExcel()
        {
            try
            {
                var query = BuildAutoFilteredQuery();
                var data = await query.ToListAsync();

                var excelData = await GenerateExcelDataAsync(data);
                var fileName = $"{typeof(T).Name}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao exportar Excel");
                return BadRequest($"Erro ao exportar: {ex.Message}");
            }
        }

        /// <summary>
        /// API: Ação customizada genérica
        /// POST: /Controller/CustomAction
        /// </summary>
        [HttpPost]
        public virtual async Task<IActionResult> CustomAction(string actionName, long[] ids, [FromBody] Dictionary<string, object>? parameters = null)
        {
            try
            {
                var result = await ExecuteCustomActionAsync(actionName, ids, parameters ?? []);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao executar ação customizada {ActionName}", actionName);
                return Json(new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// API: Duplicar registro
        /// POST: /Controller/Duplicate/{id}
        /// </summary>
        [HttpPost]
        public virtual async Task<IActionResult> Duplicate(long id)
        {
            try
            {
                var original = await GetEntityWithIncludesAsync(id);
                if (original == null)
                {
                    return NotFound();
                }

                var duplicate = await CreateDuplicateAsync(original);
                _context.Set<T>().Add(duplicate);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Registro duplicado com sucesso!",
                    newId = duplicate.Id,
                    redirectUrl = Url.Action("Edit", new { id = duplicate.Id })
                });
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro ao duplicar registro {Id}", id);
                return Json(new { success = false, message = $"Erro ao duplicar: {ex.Message}" });
            }
        }

        /// <summary>
        /// View: Configuração da entidade (para desenvolvedores)
        /// GET: /Controller/EntityConfig
        /// </summary>
        [HttpGet]
        public virtual IActionResult EntityConfig()
        {
            var config = new
            {
                EntityName = typeof(T).Name,
                Properties = _metadata.AllProperties.Select(p => new
                {
                    Name = p.Name,
                    Type = p.PropertyType.Name,
                    FormField = p.GetCustomAttribute<FormFieldAttribute>(),
                    GridAttribute = p.GetGridAttribute(),
                    IsSearchable = _metadata.SearchableProperties.Contains(p),
                    IsNavigation = _metadata.NavigationProperties.Contains(p)
                }),
                FormConfig = _metadata.FormConfig,
                FormTabs = _metadata.FormTabs,
                Tabs = _metadata.GetTabs()
            };

            return Json(config);
        }

        #endregion

        #region Virtual Methods for Override

        /// <summary>
        /// Busca entidades de referência - Override para customizar
        /// </summary>
        protected virtual async Task<object> SearchReferenceEntitiesAsync(Type referenceType, string term, int page, int pageSize)
        {
            // Implementação genérica usando reflexão
            var dbSet = _context.GetType().GetMethod("Set", Type.EmptyTypes)?.MakeGenericMethod(referenceType)?.Invoke(_context, null);

            // TODO: Implementar busca genérica
            return new { items = new List<object>(), totalCount = 0 };
        }

        /// <summary>
        /// Valida propriedade específica - Override para customizar
        /// </summary>
        protected virtual async Task<(bool IsValid, string? ErrorMessage)> ValidatePropertyAsync(PropertyInfo property, string value, T entity)
        {
            // Implementação de validação genérica
            return (true, null);
        }

        /// <summary>
        /// Obtém opções dependentes para selects - Override para customizar
        /// </summary>
        protected virtual async Task<List<SelectListItem>> GetDependentSelectOptionsAsync(string parentProperty, string parentValue, string targetProperty)
        {
            return [];
        }

        /// <summary>
        /// Gera dados para Excel - Override para customizar
        /// </summary>
        protected virtual async Task<byte[]> GenerateExcelDataAsync(List<T> data)
        {
            // Implementação básica - usar biblioteca Excel
            return Array.Empty<byte>();
        }

        /// <summary>
        /// Executa ação customizada - Override para customizar
        /// </summary>
        protected virtual async Task<object> ExecuteCustomActionAsync(string actionName, long[] ids, Dictionary<string, object> parameters)
        {
            return new { success = false, message = "Ação não implementada" };
        }

        /// <summary>
        /// Cria duplicata de registro - Override para customizar
        /// </summary>
        protected virtual async Task<T> CreateDuplicateAsync(T original)
        {
            var duplicate = new T();

            // Copiar propriedades básicas (exceto chaves e audit)
            var propertiesToCopy = _metadata.AllProperties
                .Where(p => p.CanWrite &&
                           !new[] { "Id", "DataCadastro", "DataAlteracao", "CriadoPorUsuarioId", "AlteradoPorUsuarioId" }.Contains(p.Name))
                .ToList();

            foreach (var prop in propertiesToCopy)
            {
                var value = prop.GetValue(original);
                prop.SetValue(duplicate, value);
            }

            // Configurar campos de auditoria
            duplicate.IdEmpresa = GetCurrentEmpresaId();
            duplicate.DataCadastro = DateTime.UtcNow;
            duplicate.CriadoPorUsuarioId = GetCurrentUserId();

            return duplicate;
        }

        /// <summary>
        /// Obtém ID do usuário atual - Override para implementar
        /// </summary>
        protected virtual long GetCurrentUserId()
        {
            // TODO: Implementar obtenção do usuário atual
            return 1; // Placeholder
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Obtém atributo de grid de uma propriedade
        /// </summary>
        protected object? GetGridAttribute(PropertyInfo property)
        {
            var gridAttributes = property.GetCustomAttributes()
                .Where(attr => attr.GetType().Name.StartsWith("Grid"))
                .FirstOrDefault();

            return gridAttributes;
        }

        /// <summary>
        /// Formata valor do campo para string
        /// </summary>
        protected string? FormatFieldValueToString(object value, EnumFieldType fieldType, PropertyInfo property)
        {
            if (value == null)
            {
                return null;
            }

            return fieldType switch
            {
                EnumFieldType.Date => value is DateTime dt ? dt.ToString("yyyy-MM-dd") : value.ToString(),
                EnumFieldType.DateTime => value is DateTime dt2 ? dt2.ToString("yyyy-MM-ddTHH:mm") : value.ToString(),
                EnumFieldType.Number => value.ToString(),
                EnumFieldType.Currency => value is decimal dec ? dec.ToString("C") : value.ToString(),
                EnumFieldType.Percentage => value is decimal perc ? (perc * 100).ToString("F2") + "%" : value.ToString(),
                EnumFieldType.Cpf => UltraGenericController<T>.FormatCpf(value.ToString()),
                EnumFieldType.Cnpj => UltraGenericController<T>.FormatCnpj(value.ToString()),
                EnumFieldType.Cep => UltraGenericController<T>.FormatCep(value.ToString()),
                EnumFieldType.Telefone => UltraGenericController<T>.FormatTelefone(value.ToString()),
                _ => value.ToString()
            };
        }

        private static string? FormatCpf(string? cpf)
        {
            return cpf?.Length == 11 ? $"{cpf[..3]}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}" : cpf;
        }

        private static string? FormatCnpj(string? cnpj)
        {
            return cnpj?.Length == 14 ? $"{cnpj[..2]}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}" : cnpj;
        }

        private static string? FormatCep(string? cep)
        {
            return cep?.Length == 8 ? $"{cep[..5]}-{cep.Substring(5, 3)}" : cep;
        }

        private static string? FormatTelefone(string? telefone)
        {
            return telefone?.Length switch
            {
                11 => $"({telefone[..2]}) {telefone.Substring(2, 5)}-{telefone.Substring(7, 4)}",
                10 => $"({telefone[..2]}) {telefone.Substring(2, 4)}-{telefone.Substring(6, 4)}",
                _ => telefone
            };
        }

        #endregion
    }
}