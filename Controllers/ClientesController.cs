using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Extensions;
using AutoGestao.Models;
using AutoGestao.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class ClientesController(ApplicationDbContext context, IFileStorageService fileStorageService, ILogger<StandardGridController<Cliente>> logger) 
        : StandardGridController<Cliente>(context, fileStorageService, logger)
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
                    ];

            standardGridViewModel.RowActions.AddRange(
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

            return standardGridViewModel;
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
                        query = ApplyEnumFilter(query, filters, filter.Key, c => c.TipoPessoa);
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
            var temVendas = _context.Vendas.Any(v => v.IdCliente == entity.Id);

            return temVendas
                ? throw new InvalidOperationException("Não é possível excluir cliente com vendas associadas")
                : base.BeforeDelete(entity);
        }

        protected override bool CanEdit(Cliente entity)
        {
            return entity.Ativo;
        }

        protected override bool CanDelete(Cliente entity)
        {
            var temVendas = _context.Vendas.Any(v => v.IdCliente == entity.Id);
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
            if (entity.TipoPessoa == EnumTipoPessoa.PessoaFisica && string.IsNullOrEmpty(entity.Cpf))
            {
                ModelState.AddModelError(nameof(entity.Cpf), "CPF é obrigatório para Pessoa Física");
            }

            if (entity.TipoPessoa == EnumTipoPessoa.PessoaJuridica && string.IsNullOrEmpty(entity.Cnpj))
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
    }
}