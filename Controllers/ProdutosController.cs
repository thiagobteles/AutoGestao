using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    [Authorize]
    public class ProdutosController(ApplicationDbContext context) : StandardGridController<Produto>(context)
    {
        protected override IQueryable<Produto> GetBaseQuery()
        {
            return _context.Produtos.AsQueryable();
        }

        protected override List<SelectListItem> GetSelectOptions(string propertyName)
        {
            return propertyName switch
            {
                nameof(Produto.Categoria) => Enum.GetValues<EnumCategoriaProduto>()
                    .Select(e => new SelectListItem
                    {
                        Value = e.ToString(),
                        Text = GetCategoriaDisplayName(e)
                    }).ToList(),
                _ => base.GetSelectOptions(propertyName)
            };
        }

        [Authorize(Roles = "Admin,Gerente,Vendedor")] // Apenas estes perfis podem criar
        public override async Task<IActionResult> Create()
        {
            return await base.Create();
        }

        [Authorize(Roles = "Admin,Gerente")] // Apenas Admin e Gerente podem editar
        public override async Task<IActionResult> Edit(int id)
        {
            return await base.Edit(id);
        }

        [Authorize(Roles = "Admin")] // Apenas Admin pode deletar
        public override async Task<IActionResult> Delete(int id)
        {
            return await base.Delete(id);
        }

        protected override Task BeforeCreate(Produto entity)
        {
            entity.DataCadastro = DateTime.UtcNow;

            // Gerar código automático se não informado
            if (string.IsNullOrEmpty(entity.Codigo))
            {
                entity.Codigo = GerarCodigoProduto();
            }

            ValidateProduto(entity);
            return base.BeforeCreate(entity);
        }

        protected override Task BeforeUpdate(Produto entity)
        {
            entity.DataAlteracao = DateTime.UtcNow;
            ValidateProduto(entity);
            return base.BeforeUpdate(entity);
        }

        protected override Task AfterCreate(Produto entity)
        {
            TempData["Success"] = $"Produto {entity.Nome} criado com sucesso!";
            return base.AfterCreate(entity);
        }

        protected override Task AfterUpdate(Produto entity)
        {
            TempData["Success"] = $"Produto {entity.Nome} atualizado com sucesso!";
            return base.AfterUpdate(entity);
        }

        protected override bool CanDelete(Produto entity)
        {
            // Não permitir deletar se tiver vendas associadas
            return !_context.ItensVenda.Any(i => i.ProdutoId == entity.Id);
        }

        protected override Task BeforeDelete(Produto entity)
        {
            return _context.ItensVenda.Any(i => i.ProdutoId == entity.Id)
                ? throw new InvalidOperationException("Não é possível excluir produto com vendas associadas")
                : base.BeforeDelete(entity);
        }

        protected override void ConfigureFormFields(List<FormFieldViewModel> fields, Produto entity, string action)
        {
            if (action == "Details")
            {
                // Adicionar campos calculados em modo visualização
                var margemField = new FormFieldViewModel
                {
                    PropertyName = "MargemLucro",
                    DisplayName = "Margem de Lucro",
                    Value = entity.PrecoCusto > 0 ?
                        $"{((entity.PrecoVenda - entity.PrecoCusto.Value) / entity.PrecoCusto.Value * 100):F2}%" :
                        "N/A",
                    ReadOnly = true,
                    Section = "Informações Calculadas",
                    Icon = "fas fa-percentage"
                };
                fields.Add(margemField);

                // Status do estoque
                var statusEstoque = entity.EstoqueAtual <= entity.EstoqueMinimo ? "Baixo" :
                                   entity.EstoqueAtual >= entity.EstoqueMaximo ? "Alto" : "Normal";

                var statusField = new FormFieldViewModel
                {
                    PropertyName = "StatusEstoque",
                    DisplayName = "Status do Estoque",
                    Value = statusEstoque,
                    ReadOnly = true,
                    Section = "Informações Calculadas",
                    Icon = "fas fa-chart-line"
                };
                fields.Add(statusField);
            }

            // Destacar campos obrigatórios
            var codigoField = fields.FirstOrDefault(f => f.PropertyName == nameof(Produto.Codigo));
            if (codigoField != null && action == "Create")
            {
                codigoField.Placeholder = "Será gerado automaticamente se não informado";
            }
        }

        private void ValidateProduto(Produto entity)
        {
            // Validar código único
            var codigoExistente = _context.Produtos
                .Any(p => p.Id != entity.Id && p.Codigo == entity.Codigo);
            if (codigoExistente)
            {
                ModelState.AddModelError(nameof(entity.Codigo), "Código já cadastrado");
            }

            // Validar preços
            if (entity.PrecoCusto.HasValue && entity.PrecoVenda <= entity.PrecoCusto)
            {
                ModelState.AddModelError(nameof(entity.PrecoVenda), "Preço de venda deve ser maior que o preço de custo");
            }

            // Validar estoques
            if (entity.EstoqueMinimo >= entity.EstoqueMaximo)
            {
                ModelState.AddModelError(nameof(entity.EstoqueMaximo), "Estoque máximo deve ser maior que o mínimo");
            }
        }

        private string GerarCodigoProduto()
        {
            var ultimoCodigo = _context.Produtos
                .Where(p => p.Codigo.StartsWith("PROD"))
                .OrderByDescending(p => p.Codigo)
                .Select(p => p.Codigo)
                .FirstOrDefault();

            if (ultimoCodigo == null)
            {
                return "PROD001";
            }

            var numero = int.Parse(ultimoCodigo.Substring(4)) + 1;
            return $"PROD{numero:D3}";
        }

        private static string GetCategoriaDisplayName(EnumCategoriaProduto categoria)
        {
            return categoria switch
            {
                EnumCategoriaProduto.Acessorios => "Acessórios",
                EnumCategoriaProduto.Pecas => "Peças",
                EnumCategoriaProduto.Servicos => "Serviços",
                EnumCategoriaProduto.Consumiveis => "Consumíveis",
                EnumCategoriaProduto.Ferramentas => "Ferramentas",
                _ => categoria.ToString()
            };
        }

        // Ação personalizada para relatório de estoque baixo
        [HttpGet]
        [Authorize(Roles = "Admin,Gerente")]
        public async Task<IActionResult> EstoqueBaixo()
        {
            var produtos = await _context.Produtos
                .Where(p => p.Ativo && p.EstoqueAtual <= p.EstoqueMinimo)
                .OrderBy(p => p.Nome)
                .ToListAsync();

            ViewBag.Title = "Produtos com Estoque Baixo";
            return View("Index", produtos);
        }

        protected override StandardGridViewModel ConfigureGrid()
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<Produto> ApplyFilters(IQueryable<Produto> query, Dictionary<string, object> filters)
        {
            throw new NotImplementedException();
        }

        protected override IQueryable<Produto> ApplySort(IQueryable<Produto> query, string orderBy, string orderDirection)
        {
            throw new NotImplementedException();
        }
    }
}