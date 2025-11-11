using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Models;
using AutoGestao.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers.Veiculos
{
    public class VeiculoMarcaModeloController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<VeiculoMarcaModelo>> logger) 
        : StandardGridController<VeiculoMarcaModelo>(context, fileStorageService, logger)
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
                        Placeholder = "Informe uma Marca ou Modelo para filtrar..."
                    }
                ];

            return standardGridViewModel;
        }

        protected override IQueryable<VeiculoMarcaModelo> ApplyFilters(IQueryable<VeiculoMarcaModelo> query, Dictionary<string, object> filters)
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
                                c => c.Descricao,
                                c => c.VeiculoMarca.Descricao);
                        }
                        break;
                }
            }

            return query;
        }

        protected override async Task<bool> CanCreate(VeiculoMarcaModelo? entity)
        {
            // 🔧 FIX: Setar CurrentEmpresaId no contexto para Query Filter Global
            _context.CurrentEmpresaId = GetCurrentEmpresaId();

            // Verificar se já existe um modelo com a mesma descrição para a mesma marca
            var exists = await _context.VeiculoMarcaModelos.AnyAsync(x => x.Descricao == entity.Descricao && x.IdVeiculoMarca == entity.IdVeiculoMarca);
            if (exists)
            {
                ModelState.AddModelError(nameof(entity.Descricao), "Este modelo já está cadastrado para a marca selecionada!");
            }

            return await base.CanCreate(entity);
        }
    }
}