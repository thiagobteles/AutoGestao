using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Extensions;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class ClientesController(ApplicationDbContext context) : StandardGridController<Cliente>(context)
    {
        protected override IQueryable<Cliente> GetBaseQuery()
        {
            return _context.Clientes.AsQueryable();
        }

        protected override StandardGridViewModel ConfigureGrid()
        {
            var retorno = new StandardGridViewModel("Clientes", "Gerencie todos os clientes", "Clientes")
            {
                Filters =
                    [
                        new()
                        {
                            Name = "search",
                            DisplayName = "Busca Geral",
                            Type = GridFilterType.Text,
                            Placeholder = "Nome, CPF, CNPJ, Email, Telefone..."
                        },
                        new()
                        {
                            Name = "tipocliente",
                            DisplayName = "Tipo de Pessoa",
                            Type = GridFilterType.Select,
                            Placeholder = "Tipo Pessoa...",
                            Options = GetTipoClienteOptions()
                        },
                        new()
                        {
                            Name = "status",
                            DisplayName = "Status",
                            Type = GridFilterType.Select,
                            Placeholder = "Status cliente...",
                            Options =
                            [
                                new() { Value = "true", Text = "✅ Ativo" },
                                new() { Value = "false", Text = "❌ Inativo" }
                            ]
                        }
                    ],

                Columns =
                    [
                        new() { Name = nameof(Cliente.Id), DisplayName = "Cód", Type = GridColumnType.Text, Sortable = true, Width = "65px" },
                        new() { Name = nameof(Cliente.TipoCliente), DisplayName = "Tipo", Type = GridColumnType.Enumerador, EnumRender = EnumRenderType.Icon, Sortable = true, Width = "65px"},
                        new() { Name = nameof(Cliente.Nome), DisplayName = "Nome/Razão Social", Sortable = true, Type = GridColumnType.Text, UrlAction = "Details" },
                        new() { Name = "Documento", DisplayName = "CPF/CNPJ", Sortable = true, Type = GridColumnType.Custom, CustomRender = RenderDocumento},
                        new() { Name = nameof(Cliente.Celular), DisplayName = "Telefone", Sortable = true },
                        new() { Name = nameof(Cliente.Cidade), DisplayName = "Cidade", Sortable = true },
                        new() { Name = nameof(Cliente.Estado), DisplayName = "UF", Type = GridColumnType.Enumerador, EnumRender = EnumRenderType.Description, Sortable = true, Width = "65px"},
                        new() { Name = nameof(Cliente.Ativo), DisplayName = "Ativo", Type = GridColumnType.Enumerador, Sortable = true, Width = "65px" },
                        new() { Name = "Actions", DisplayName = "Ações", Type = GridColumnType.Actions, Sortable = false, Width = "100px" }
                    ]
            };

            retorno.RowActions.AddRange(
                [
                    new()
                    {
                        Name = "NewSale",
                        DisplayName = "Nova Venda",
                        Icon = "fas fa-handshake",
                        Url = "/Vendas/Create?clienteId={id}",
                        ShowCondition = (x) => ((Cliente)x).Ativo == true
                    },
                    new()
                    {
                        Name = "NewEvaluation",
                        DisplayName = "Nova Avaliação",
                        Icon = "fas fa-clipboard-check",
                        Url = "/Avaliacoes/Create?clienteId={id}",
                        ShowCondition = (x) => ((Cliente)x).Ativo == true
                    },
                    new()
                    {
                        Name = "ToggleStatus",
                        DisplayName = "Inativar",
                        Icon = "fas fa-ban",
                        Url = "/Clientes/ToggleStatus/{id}",
                        ShowCondition = (x) => ((Cliente)x).Ativo == true
                    },
                    new()
                    {
                        Name = "ToggleStatus",
                        DisplayName = "Ativar",
                        Icon = "fas fa-check",
                        Url = "/Clientes/ToggleStatus/{id}",
                        ShowCondition = (item) => ((Cliente)item).Ativo == false
                    }
                ]);

            return retorno;
        }

        protected override IQueryable<Cliente> ApplyFilters(IQueryable<Cliente> query, Dictionary<string, object> filters)
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
                                c => c.CPF,
                                c => c.CNPJ,
                                c => c.Email,
                                c => c.Telefone,
                                c => c.Celular);
                        }
                        break;

                    case "codigo":
                        if (int.TryParse(filter.Value.ToString(), out int codigo))
                        {
                            query = query.Where(c => c.Id == codigo);
                        }
                        break;

                    case "status":
                        if (bool.TryParse(filter.Value.ToString(), out bool status))
                        {
                            query = query.Where(c => c.Ativo == status);
                        }
                        break;

                    case "tipocliente":
                        query = ApplyEnumFilter(query, filters, filter.Key, c => c.TipoCliente);
                        break;

                    case "estado":
                        query = ApplyEnumFilter(query, filters, filter.Key, c => c.Estado);
                        break;

                    case "cidade":
                        var cidade = filter.Value.ToString();
                        if (!string.IsNullOrEmpty(cidade))
                        {
                            query = query.Where(c => c.Cidade != null && c.Cidade.Contains(cidade));
                        }
                        break;

                    case "datacadastro":
                        if (DateTime.TryParse(filter.Value.ToString(), out DateTime dataCadastro))
                        {
                            query = query.Where(c => c.DataCadastro.Date == dataCadastro.Date);
                        }
                        break;

                    case "periodo_inicio":
                    case "periodo_fim":
                        // Será tratado pelo helper ApplyDateRangeFilter
                        break;
                }
            }

            // Aplicar filtro de período usando o helper
            query = ApplyDateRangeFilter(query, filters, "periodo", c => c.DataCadastro);

            return query;
        }

        protected override IQueryable<Cliente> ApplySort(IQueryable<Cliente> query, string orderBy, string orderDirection)
        {
            return orderBy?.ToLower() switch
            {
                "id" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Id)
                    : query.OrderBy(c => c.Id),
                "nome" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Nome)
                    : query.OrderBy(c => c.Nome),
                "tipocliente" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.TipoCliente)
                    : query.OrderBy(c => c.TipoCliente),
                "cpf" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.CPF)
                    : query.OrderBy(c => c.CPF),
                "cidade" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Cidade)
                    : query.OrderBy(c => c.Cidade),
                "estado" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Estado)
                    : query.OrderBy(c => c.Estado),
                "datacadastro" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.DataCadastro)
                    : query.OrderBy(c => c.DataCadastro),
                "ativo" => orderDirection == "desc"
                    ? query.OrderByDescending(c => c.Ativo)
                    : query.OrderBy(c => c.Ativo),
                _ => query.OrderByDescending(c => c.DataCadastro) // Default: Últimos cadastros
            };
        }

        protected override List<SelectListItem> GetSelectOptions(string propertyName)
        {
            return propertyName switch
            {
                nameof(Cliente.TipoCliente) =>
                [
                    new() { Value = "PessoaFisica", Text = "👤 Pessoa Física" },
                    new() { Value = "PessoaJuridica", Text = "🏢 Pessoa Jurídica" }
                ],
                nameof(Cliente.Estado) => GetEstadosOptions(),
                _ => base.GetSelectOptions(propertyName)
            };
        }

        protected override Task BeforeCreate(Cliente entity)
        {
            entity.DataCadastro = DateTime.UtcNow;
            ValidateCliente(entity);
            return base.BeforeCreate(entity);

        }

        protected override Task AfterCreate(Cliente entity)
        {
            // Log da criação
            TempData["Success"] = $"Cliente {entity.Nome} criado com sucesso!";
            return base.AfterCreate(entity);
        }

        protected override Task BeforeUpdate(Cliente entity)
        {
            entity.DataAlteracao = DateTime.UtcNow;
            ValidateCliente(entity);
            return base.BeforeUpdate(entity);

        }

        protected override Task AfterUpdate(Cliente entity)
        {
            TempData["Success"] = $"Cliente {entity.Nome} atualizado com sucesso!";
            return base.AfterUpdate(entity);

        }

        protected override Task BeforeDelete(Cliente entity)
        {
            // Verificar se pode deletar
            var temVendas = _context.Vendas.Any(v => v.ClienteId == entity.Id);
            return temVendas
                ? throw new InvalidOperationException("Não é possível excluir cliente com vendas associadas")
                : base.BeforeDelete(entity);
        }

        // Só pode editar clientes ativos
        protected override bool CanEdit(Cliente entity)
        {
            return entity.Ativo; 
        }

        // Só pode deletar clientes que não estão vinculados a compras
        protected override bool CanDelete(Cliente entity)
        {
            var temVendas = _context.Vendas.Any(v => v.ClienteId == entity.Id);
            return !temVendas;
        }

        protected override void ConfigureFormFields(List<FormFieldViewModel> fields, Cliente entity, string action)
        {
            if (action == "Details")
            {
                // Adicionar campos calculados em modo visualização
                var nomeField = fields.FirstOrDefault(f => f.PropertyName == nameof(Cliente.Nome));
                if (nomeField != null)
                {
                    nomeField.DisplayName = $"Nome do Cliente (#{entity.Id})";
                }
            }
        }

        #region Ações Específicas

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Ativo = !cliente.Ativo;
                cliente.DataAlteracao = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Cliente {(cliente.Ativo ? "ativado" : "inativado")} com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Cliente não encontrado!";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            try
            {
                var clientes = await _context.Clientes
                    .OrderBy(c => c.Nome)
                    .ToListAsync();

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("ID,Tipo,Nome,CPF/CNPJ,Email,Telefone,Celular,Cidade,Estado,Status,Data Cadastro");

                foreach (var cliente in clientes)
                {
                    var documento = !string.IsNullOrEmpty(cliente.CPF) ? cliente.CPF : cliente.CNPJ;
                    csv.AppendLine($"{cliente.Id}," +
                                  $"{cliente.TipoCliente.GetDescription()}," +
                                  $"\"{cliente.Nome}\"," +
                                  $"{documento}," +
                                  $"{cliente.Email}," +
                                  $"{cliente.Telefone}," +
                                  $"{cliente.Celular}," +
                                  $"{cliente.Cidade}," +
                                  $"{cliente.Estado.GetDescription()}," +
                                  $"{(cliente.Ativo ? "Ativo" : "Inativo")}," +
                                  $"{cliente.DataCadastro:dd/MM/yyyy}");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var fileName = $"clientes_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao exportar dados: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Import()
        {
            TempData["ErrorMessage"] = $"Operação ainda não implementada!";
            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Métodos Auxiliares para Filtros

        private static List<SelectListItem> GetTipoClienteOptions()
        {
            var options = new List<SelectListItem>();
            var enumDictionary = EnumExtension.GetEnumDictionary<EnumTipoPessoa>();

            foreach (var item in enumDictionary.Where(x => x.Key != 0))
            {
                options.Add(new SelectListItem
                {
                    Value = ((EnumTipoPessoa)item.Key).ToString(),
                    Text = $"{((EnumTipoPessoa)item.Key).GetIcone()} {item.Value}"
                });
            }

            return options;
        }

        private void ValidateCliente(Cliente entity)
        {
            // Validações específicas
            if (entity.TipoCliente == EnumTipoPessoa.PessoaFisica && string.IsNullOrEmpty(entity.CPF))
            {
                ModelState.AddModelError(nameof(entity.CPF), "CPF é obrigatório para Pessoa Física");
            }

            if (entity.TipoCliente == EnumTipoPessoa.PessoaJuridica && string.IsNullOrEmpty(entity.CNPJ))
            {
                ModelState.AddModelError(nameof(entity.CNPJ), "CNPJ é obrigatório para Pessoa Jurídica");
            }

            // Verificar CPF único
            if (!string.IsNullOrEmpty(entity.CPF))
            {
                var cpfExistente = _context.Clientes.Any(c => c.Id != entity.Id && c.CPF == entity.CPF);
                if (cpfExistente)
                {
                    ModelState.AddModelError(nameof(entity.CPF), "CPF já cadastrado");
                }
            }
        }

        private List<SelectListItem> GetEstadosOptions()
        {
            return
            [
                new() { Value = "", Text = "Selecione o estado..." },         new() { Value = "SP", Text = "São Paulo" },         new() { Value = "RJ", Text = "Rio de Janeiro" },         new() { Value = "MG", Text = "Minas Gerais" },         new() { Value = "RS", Text = "Rio Grande do Sul" },         // ... adicionar todos os estados
            ];
        }

        private static string RenderDocumento(object item)
        {
            var cliente = (Cliente)item;
            var documento = cliente.TipoCliente == EnumTipoPessoa.PessoaJuridica
                ? cliente.CNPJ.AplicarMascaraCnpj()
                : cliente.CPF.AplicarMascaraCpf();

            return $@"{documento}";
        }

        #endregion
    }
}