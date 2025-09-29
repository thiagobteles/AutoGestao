using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class ClientesController(ApplicationDbContext context) : StandardGridController<Cliente>(context)
    {
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
                            Type = EnumGridFilterType.Text,
                            Placeholder = "Nome, CPF, CNPJ, Email, Telefone..."
                        },
                        new()
                        {
                            Name = "tipocliente",
                            DisplayName = "Tipo de Pessoa",
                            Type = EnumGridFilterType.Select,
                            Placeholder = "Tipo Pessoa...",
                            Options = EnumExtension.GetSelectListItems<EnumTipoPessoa>(true)
                        },
                        new()
                        {
                            Name = "status",
                            DisplayName = "Status",
                            Type = EnumGridFilterType.Select,
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
                        new() { Name = nameof(Cliente.Id), DisplayName = "Cód", Type = EnumGridColumnType.Text, Sortable = true, Width = "65px" },
                        new() { Name = nameof(Cliente.TipoCliente), DisplayName = "Tipo", Type = EnumGridColumnType.Enumerador, EnumRender = EnumRenderType.Icon, Sortable = true, Width = "65px"},
                        new() { Name = nameof(Cliente.Nome), DisplayName = "Nome/Razão Social", Sortable = true, Type = EnumGridColumnType.Text, UrlAction = "Details" },
                        new() { Name = "Documento", DisplayName = "CPF/CNPJ", Sortable = true, Type = EnumGridColumnType.Custom, CustomRender = RenderDocumento},
                        new() { Name = nameof(Cliente.Celular), DisplayName = "Telefone", Sortable = true },
                        new() { Name = nameof(Cliente.Cidade), DisplayName = "Cidade", Sortable = true },
                        new() { Name = nameof(Cliente.Estado), DisplayName = "UF", Type = EnumGridColumnType.Enumerador, EnumRender = EnumRenderType.Description, Sortable = true, Width = "65px"},
                        new() { Name = nameof(Cliente.Ativo), DisplayName = "Ativo", Type = EnumGridColumnType.Enumerador, Sortable = true, Width = "65px" },
                        new() { Name = "Actions", DisplayName = "Ações", Type = EnumGridColumnType.Actions, Sortable = false, Width = "100px" }
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
                        Name = "AlterarStatus",
                        DisplayName = "Inativar",
                        Icon = "fas fa-ban",
                        Url = "/Clientes/AlterarStatus/{id}",
                        ShowCondition = (x) => ((Cliente)x).Ativo == true
                    },
                    new()
                    {
                        Name = "AlterarStatus",
                        DisplayName = "Ativar",
                        Icon = "fas fa-check",
                        Url = "/Clientes/AlterarStatus/{id}",
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
                                c => c.Cpf,
                                c => c.Cnpj,
                                c => c.Email,
                                c => c.Telefone,
                                c => c.Celular);
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
                }
            }

            return query;
        }

        protected override Task BeforeCreate(Cliente entity)
        {
            ValidarCliente(entity);
            return base.BeforeCreate(entity);
        }

        protected override Task AfterCreate(Cliente entity)
        {
            TempData["Success"] = $"Cliente {entity.Nome} criado com sucesso!";
            return base.AfterCreate(entity);
        }

        protected override Task BeforeUpdate(Cliente entity)
        {
            ValidarCliente(entity);
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

        private void ValidarCliente(Cliente entity)
        {
            // Validações específicas
            if (entity.TipoCliente == EnumTipoPessoa.PessoaFisica && string.IsNullOrEmpty(entity.Cpf))
            {
                ModelState.AddModelError(nameof(entity.Cpf), "CPF é obrigatório para Pessoa Física");
            }

            if (entity.TipoCliente == EnumTipoPessoa.PessoaJuridica && string.IsNullOrEmpty(entity.Cnpj))
            {
                ModelState.AddModelError(nameof(entity.Cnpj), "CNPJ é obrigatório para Pessoa Jurídica");
            }

            // Verificar CPF único
            if (!string.IsNullOrEmpty(entity.Cpf))
            {
                var cpfExistente = _context.Clientes.Any(c => c.Id != entity.Id && c.Cpf == entity.Cpf);
                if (cpfExistente)
                {
                    ModelState.AddModelError(nameof(entity.Cpf), "CPF já cadastrado");
                }
            }
        }

        private static string RenderDocumento(object item)
        {
            var cliente = (Cliente)item;
            var documento = cliente.TipoCliente == EnumTipoPessoa.PessoaJuridica
                ? cliente.Cnpj.AplicarMascaraCnpj()
                : cliente.Cpf.AplicarMascaraCpf();

            return $@"{documento}";
        }

        #region ENDPOINTS ESPECÍFICOS

        [HttpPost]
        public async Task<IActionResult> AlterarStatus(int id)
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
                    var documento = !string.IsNullOrEmpty(cliente.Cpf) ? cliente.Cpf : cliente.Cnpj;
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
        public IActionResult Import()
        {
            TempData["ErrorMessage"] = $"Operação ainda não implementada!";
            return RedirectToAction(nameof(Index));
        }

        #endregion ENDPOINTS ESPECÍFICOS
    }
}