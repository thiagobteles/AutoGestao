using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Leads;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AutoGestao.Controllers
{
    public class LeadController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Lead>> logger, IReportService reportService) 
        : StandardGridController<Lead>(context, fileStorageService, reportService, logger)
    {

        protected override StandardGridViewModel ConfigureCustomGrid(StandardGridViewModel standardGridViewModel)
        {
            standardGridViewModel.Filters =
                    [
                        new()
                        {
                            Name = "search",
                            DisplayName = "Busca Geral",
                            Type = EnumGridFilterType.Text,
                            Placeholder = "Nome, Email, Celular..."
                        },
                        new()
                        {
                            Name = "tiporetorno",
                            DisplayName = "Tipo Retorno",
                            Type = EnumGridFilterType.Select,
                            Placeholder = "Tipo Retorno...",
                            Options = EnumExtension.GetSelectListItems<EnumTipoRetornoContato>(true)
                        },
                        new()
                        {
                            Name = "status",
                            DisplayName = "Status",
                            Type = EnumGridFilterType.Select,
                            Placeholder = "Status Lead...",
                            Options = EnumExtension.GetSelectListItems<EnumStatusLead>(true)
                        }
                    ];

            return standardGridViewModel;
        }

        protected override IQueryable<Lead> ApplyFilters(IQueryable<Lead> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "search":
                        var searchTerm = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(searchTerm))
                        {
                            query = ApplyTextFilter(query, searchTerm,
                                c => c.Nome,
                                c => c.Email,
                                c => c.Celular);
                        }
                        break;

                    case "tiporetorno":
                        query = ApplyEnumFilter(query, filters, filter.Key, c => c.TipoRetornoContato);
                        break;

                    case "status":
                        query = ApplyEnumFilter(query, filters, filter.Key, c => c.Status);
                        break;
                }
            }

            return query;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Create(Lead entity)
        {
            if (!CanCreate(entity))
            {
                TempData["Error"] = "Você não tem permissão para cadastrar um novo registro.";
                return Forbid();
            }

            var allProperties = typeof(Lead).GetProperties();
            if (IsAjaxRequest())
            {
                return await HandleModalCreate(this, entity);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await BeforeCreate(entity);
                    _context.Set<Lead>().Add(entity);
                    await _context.SaveChangesAsync();
                    await AfterCreate(entity);

                    if (Request.IsAjaxRequest())
                    {
                        return Json(new { success = true, message = "Registro criado com sucesso!", redirectUrl = Url.Action("Index") });
                    }

                    TempData["Success"] = "Registro criado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Erro ao criar registro: {ex.Message}");
                }
            }
            else
            {
                foreach (var error in ModelState)
                {
                    if (error.Value.Errors.Any())
                    {
                        Console.WriteLine($"{error.Key}: {string.Join(", ", error.Value.Errors.Select(e => e.ErrorMessage))}");
                    }
                }
            }

            var viewModel = await BuildFormViewModelAsync(entity, "Create");
            AddModelStateToViewModel(viewModel);

            return Request.IsAjaxRequest()
                ? PartialView("_StandardFormContent", viewModel)
                : View("_StandardForm", viewModel);
        }
    }
}