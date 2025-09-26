using AutoGestao.Controllers;
using AutoGestao.Entidades;
using Microsoft.AspNetCore.Mvc;

namespace AutoGestao.Extensions
{
    public static class StandardGridControllerReferenceExtensions
    {
        ///// <summary>
        ///// Manipula criação via modal para campos de referência
        ///// </summary>
        ///// <typeparam name="T">Tipo da entidade</typeparam>
        ///// <param name="controller">Controller</param>
        ///// <param name="entity">Entidade a ser criada</param>
        ///// <returns>ActionResult apropriado (JSON para AJAX, View para navegação normal)</returns>
        //public static async Task<IActionResult> HandleModalCreate<T>(
        //    this StandardGridController<T> controller,
        //    T entity) where T : BaseEntidade, new()
        //{
        //    // Verifica se é requisição AJAX (modal)
        //    if (controller.Request.Headers.ContainsKey("X-Requested-With") &&
        //        controller.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        //    {
        //        try
        //        {
        //            if (controller.ModelState.IsValid)
        //            {
        //                // Executar lógica de criação
        //                await controller.BeforeCreate(entity);

        //                controller.DbContext.Set<T>().Add(entity);
        //                await controller.DbContext.SaveChangesAsync();

        //                await controller.AfterCreate(entity);

        //                // Retornar JSON para o modal
        //                return controller.Json(new
        //                {
        //                    success = true,
        //                    id = entity.Id,
        //                    text = GetDisplayText(entity),
        //                    name = GetDisplayText(entity), // Compatibilidade
        //                    message = "Registro criado com sucesso!"
        //                });
        //            }
        //            else
        //            {
        //                // Retornar erros de validação
        //                var errors = controller.ModelState
        //                    .Where(x => x.Value?.Errors.Count > 0)
        //                    .ToDictionary(
        //                        kvp => kvp.Key,
        //                        kvp => string.Join("; ", kvp.Value?.Errors.Select(e => e.ErrorMessage) ?? new string[0])
        //                    );

        //                return controller.Json(new
        //                {
        //                    success = false,
        //                    errors
        //                });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            controller.Logger?.LogError(ex, "Erro ao criar {EntityType} via modal", typeof(T).Name);

        //            return controller.Json(new
        //            {
        //                success = false,
        //                errors = new Dictionary<string, string>
        //                {
        //                    ["general"] = $"Erro interno: {ex.Message}"
        //                }
        //            });
        //        }
        //    }

        //    // Comportamento normal se não for AJAX
        //    return await controller.DefaultCreate(entity);
        //}

        ///// <summary>
        ///// Obtém o texto de exibição apropriado para uma entidade
        ///// </summary>
        ///// <typeparam name="T">Tipo da entidade</typeparam>
        ///// <param name="entity">Instância da entidade</param>
        ///// <returns>Texto para exibição</returns>
        //private static string GetDisplayText<T>(T entity) where T : BaseEntidade
        //{
        //    var type = typeof(T);

        //    // Propriedades comuns para exibição, em ordem de prioridade
        //    var displayProperties = new[] { "Nome", "Descricao", "Titulo", "RazaoSocial", "Codigo", "Numero" };

        //    foreach (var propName in displayProperties)
        //    {
        //        var prop = type.GetProperty(propName);
        //        if (prop != null && prop.PropertyType == typeof(string))
        //        {
        //            var value = prop.GetValue(entity) as string;
        //            if (!string.IsNullOrWhiteSpace(value))
        //            {
        //                return value;
        //            }
        //        }
        //    }

        //    // Fallback para nome do tipo + ID
        //    var typeName = type.Name;
        //    return $"{typeName} #{entity.Id}";
        //}

        ///// <summary>
        ///// Execução padrão do Create (comportamento original)
        ///// </summary>
        ///// <typeparam name="T">Tipo da entidade</typeparam>
        ///// <param name="controller">Controller</param>
        ///// <param name="entity">Entidade</param>
        ///// <returns>ActionResult</returns>
        //private static async Task<IActionResult> DefaultCreate<T>(this StandardGridController<T> controller, T entity) where T : BaseEntidade, new()
        //{
        //    try
        //    {
        //        await controller.BeforeCreate(entity);

        //        if (controller.ModelState.IsValid)
        //        {
        //            controller._context.Set<T>().Add(entity);
        //            await controller._context.SaveChangesAsync();

        //            await controller.AfterCreate(entity);
        //            return controller.RedirectToAction("Index");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        controller.Logger?.LogError(ex, "Erro ao criar {EntityType}", typeof(T).Name);
        //        controller.ModelState.AddModelError("", $"Erro ao salvar: {ex.Message}");
        //    }

        //    // Se chegou até aqui, há erros - mostrar form novamente
        //    var viewModel = controller.BuildFormViewModel(entity, "Create");
        //    return controller.View(viewModel);
        //}
    }
}