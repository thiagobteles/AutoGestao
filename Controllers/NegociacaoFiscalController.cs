using FGT.Controllers.Base;
using FGT.Data;
using FGT.Entidades;
using FGT.Enumerador.Gerais;
using FGT.Models;
using FGT.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FGT.Controllers
{
    [Authorize(Roles = "Admin,Gerente,Financeiro")]
    public class NegociacaoFiscalController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<NegociacaoFiscal>> logger)
        : StandardGridController<NegociacaoFiscal>(context, fileStorageService, logger)
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
                    Placeholder = "Nome, CPF/CNPJ, Número da Conta..."
                },
                new()
                {
                    Name = "uf",
                    DisplayName = "UF",
                    Type = EnumGridFilterType.Text,
                    Placeholder = "Filtrar por UF..."
                },
                new()
                {
                    Name = "situacao",
                    DisplayName = "Situação",
                    Type = EnumGridFilterType.Text,
                    Placeholder = "Filtrar por situação..."
                }
            ];

            return standardGridViewModel;
        }

        protected override IQueryable<NegociacaoFiscal> ApplyFilters(IQueryable<NegociacaoFiscal> query, Dictionary<string, object> filters)
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
                                n => n.NomeOptante,
                                n => n.CpfCnpjOptante,
                                n => n.NumeroContaNegociacao);
                        }
                        break;

                    case "uf":
                        var uf = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(uf))
                        {
                            query = query.Where(n => n.UFOptante == uf);
                        }
                        break;

                    case "situacao":
                        var situacao = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(situacao))
                        {
                            query = query.Where(n => n.SituacaoNegociacao == situacao);
                        }
                        break;
                }
            }

            return query;
        }

        protected override Task AfterCreate(NegociacaoFiscal entity)
        {
            TempData["NotificationScript"] = $"showSuccess('Negociação fiscal cadastrada com sucesso!')";
            return base.AfterCreate(entity);
        }

        protected override Task AfterUpdate(NegociacaoFiscal entity)
        {
            TempData["NotificationScript"] = $"showSuccess('Negociação fiscal atualizada com sucesso!')";
            return base.AfterUpdate(entity);
        }
    }
}
