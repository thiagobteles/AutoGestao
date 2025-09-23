using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    [Authorize(Roles = "Admin,Gerente,Vendedor")]
    public class ItensVendaController(ApplicationDbContext context) : StandardGridController<ItemVenda>(context)
    {
        protected override IQueryable<ItemVenda> GetBaseQuery()
        {
            return _context.ItensVenda
                .Include(i => i.Venda)
                .Include(i => i.Produto)
                .AsQueryable();
        }

        protected override List<SelectListItem> GetSelectOptions(string propertyName)
        {
            return propertyName switch
            {
                nameof(ItemVenda.VendaId) => _context.Vendas
                    .Select(v => new SelectListItem
                    {
                        Value = v.Id.ToString(),
                        Text = $"Venda #{v.Id} - {v.Cliente.Nome}"
                    })
                    .ToList(),
                nameof(ItemVenda.ProdutoId) => _context.Produtos
                    .Where(p => p.Ativo)
                    .Select(p => new SelectListItem
                    {
                        Value = p.Id.ToString(),
                        Text = $"{p.Codigo} - {p.Nome}"
                    })
                    .ToList(),
                _ => base.GetSelectOptions(propertyName)
            };
        }

        protected override Task BeforeCreate(ItemVenda entity)
        {
            entity.DataCadastro = DateTime.UtcNow;
            entity.CalcularValores();
            ValidateItemVenda(entity);
            return base.BeforeCreate(entity);
        }

        protected override Task BeforeUpdate(ItemVenda entity)
        {
            entity.DataAlteracao = DateTime.UtcNow;
            entity.CalcularValores();
            ValidateItemVenda(entity);
            return base.BeforeUpdate(entity);
        }

        protected override void ConfigureFormFields(List<FormFieldViewModel> fields, ItemVenda entity, string action)
        {
            // Calcular valores para exibição
            if (entity != null)
            {
                entity.CalcularValores();
            }

            // Em modo de visualização, mostrar informações adicionais
            if (action == "Details" && entity != null)
            {
                // Adicionar margem de lucro se tiver preço de custo
                if (entity.Produto?.PrecoCusto.HasValue == true)
                {
                    var margemField = new FormFieldViewModel
                    {
                        PropertyName = "MargemLucro",
                        DisplayName = "Margem de Lucro",
                        Value = $"{((entity.ValorUnitario - entity.Produto.PrecoCusto.Value) / entity.Produto.PrecoCusto.Value * 100):F2}%",
                        ReadOnly = true,
                        Section = "Análise",
                        Icon = "fas fa-chart-line"
                    };
                    fields.Add(margemField);
                }
            }
        }

        private void ValidateItemVenda(ItemVenda entity)
        {
            // Verificar se o produto existe e está ativo
            var produto = _context.Produtos.Find(entity.ProdutoId);
            if (produto == null || !produto.Ativo)
            {
                ModelState.AddModelError(nameof(entity.ProdutoId), "Produto não encontrado ou inativo");
                return;
            }

            // Verificar estoque disponível
            if (produto.EstoqueAtual < entity.Quantidade)
            {
                ModelState.AddModelError(nameof(entity.Quantidade), $"Quantidade indisponível. Estoque atual: {produto.EstoqueAtual}");
            }

            // Verificar se o valor unitário não está muito abaixo do preço de custo
            if (produto.PrecoCusto.HasValue && entity.ValorUnitario < produto.PrecoCusto * 0.8m)
            {
                ModelState.AddModelError(nameof(entity.ValorUnitario), "Valor unitário muito abaixo do preço de custo");
            }
        }

        // Ação para buscar informações do produto via AJAX
        [HttpGet]
        public async Task<IActionResult> GetProdutoInfo(int produtoId)
        {
            var produto = await _context.Produtos.FindAsync(produtoId);
            return produto == null
                ? NotFound()
                : Json(new
                    {
                        nome = produto.Nome,
                        precoVenda = produto.PrecoVenda,
                        estoqueAtual = produto.EstoqueAtual,
                        estoqueMinimo = produto.EstoqueMinimo
                    });
        }

        protected override StandardGridViewModel ConfigureGrid()
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<ItemVenda> ApplyFilters(IQueryable<ItemVenda> query, Dictionary<string, object> filters)
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<ItemVenda> ApplySort(IQueryable<ItemVenda> query, string orderBy, string orderDirection)
        {
            throw new NotImplementedException();
        }
    }
}