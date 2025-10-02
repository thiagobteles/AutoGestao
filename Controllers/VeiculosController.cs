using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Enumerador.Veiculo;
using AutoGestao.Extensions;
using AutoGestao.Helpers;
using AutoGestao.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class VeiculosController(ApplicationDbContext context) : StandardGridController<Veiculo>(context)
    {
        protected override IQueryable<Veiculo> GetBaseQuery()
        {
            return _context.Veiculos
                .Include(v => v.VeiculoMarca)
                .Include(v => v.VeiculoMarcaModelo)
                .Include(v => v.Cliente)
                .OrderByDescending(v => v.Id)
                .AsQueryable();
        }

        protected override StandardGridViewModel ConfigureCustomGrid(StandardGridViewModel standardGridViewModel)
        {
            standardGridViewModel.Filters =
                [
                    new()
                    {
                        Name = "search",
                        DisplayName = "Busca Geral",
                        Type = EnumGridFilterType.Text,
                        Placeholder = "Código, Placa, Chassi, Renavam, Marca, Modelo..."
                    },
                    new()
                    {
                        Name = "situacao",
                        DisplayName = "Situação",
                        Type = EnumGridFilterType.Select,
                        Placeholder = "Situação do veiculo...",
                        Options = EnumExtension.GetSelectListItems<EnumSituacaoVeiculo>(true)
                    }
                ];

            standardGridViewModel.RowActions.AddRange(
                [
                    new()
                    {
                        Name = "Sell",
                        DisplayName = "Vender",
                        Icon = "fas fa-handshake",
                        Url = "/Vendas/Create?idVeiculo={id}",
                        ShowCondition = (x) => ((Veiculo)x).Situacao == EnumSituacaoVeiculo.Estoque
                    },
                    new()
                    {
                        Name = "Reserve",
                        DisplayName = "Reservar",
                        Icon = "fas fa-bookmark",
                        Url = "/Veiculos/Reservar/{id}",
                        ShowCondition = (x) => ((Veiculo)x).Situacao == EnumSituacaoVeiculo.Estoque
                    },
                    new()
                    {
                        Name = "Unreserve",
                        DisplayName = "Liberar",
                        Icon = "fas fa-bookmark-o",
                        Url = "/Veiculos/Liberar/{id}",
                        ShowCondition = (x) => ((Veiculo)x).Situacao == EnumSituacaoVeiculo.Reservado
                    }
                ]);

            standardGridViewModel.RowActions.FirstOrDefault(x => x.Name == "Delete").ShowCondition = (x) => ((Veiculo)x).Situacao != EnumSituacaoVeiculo.Vendido;

            return standardGridViewModel;
        }

        protected override IQueryable<Veiculo> ApplyFilters(IQueryable<Veiculo> query, Dictionary<string, object> filters)
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
                                c => c.Codigo,
                                c => c.Placa,
                                //c => c.VeiculoMarca.Descricao,
                                //c => c.VeiculoMarcaModelo.Descricao,
                                c => c.Chassi,
                                c => c.Renavam);
                        }
                        break;

                    case "situacao":
                        query = ApplyEnumFilter(query, filters, filter.Key, v => v.Situacao);
                        break;
                }
            }

            return query;
        }

        protected override void ConfigureFormFields(List<FormFieldViewModel> fields, Veiculo entity, string action)
        {
            if (action == "Details")
            {
                // Adicionar tempo em estoque
                var tempoEstoque = DateTime.UtcNow - entity.DataCadastro;
                fields.Add(new FormFieldViewModel
                {
                    PropertyName = "TempoEstoque",
                    DisplayName = "Tempo em Estoque",
                    Value = $"{tempoEstoque.Days} dias",
                    ReadOnly = true,
                    Section = "Informações Adicionais"
                });
            }
        }

        protected override Task BeforeCreate(Veiculo entity)
        {
            entity.DataCadastro = DateTime.UtcNow;
            entity.Situacao = EnumSituacaoVeiculo.Estoque;

            if (string.IsNullOrEmpty(entity.Codigo))
            {
                entity.Codigo = GerarCodigoVeiculo();
            }

            ValidarVeiculo(entity);
            return base.BeforeCreate(entity);
        }

        protected override Task BeforeUpdate(Veiculo entity)
        {
            entity.DataAlteracao = DateTime.UtcNow;
            ValidarVeiculo(entity);
            return base.BeforeUpdate(entity);
        }

        protected override bool CanDelete(Veiculo entity)
        {
            return entity.Situacao != EnumSituacaoVeiculo.Vendido;
        }

        private void ValidarVeiculo(Veiculo entity)
        {
            // Validar placa única
            var placaExistente = _context.Veiculos.Any(v => v.Id != entity.Id && v.Placa == entity.Placa);
            if (placaExistente)
            {
                ModelState.AddModelError(nameof(entity.Placa), "Placa já cadastrada");
            }

            // Validar anos
            if (entity.AnoModelo < entity.AnoFabricacao)
            {
                ModelState.AddModelError(nameof(entity.AnoModelo), "Ano do modelo não pode ser menor que ano de fabricação");
            }
        }

        private string GerarCodigoVeiculo()
        {
            var ultimoCodigo = _context.Veiculos
                .OrderByDescending(v => v.Id)
                .Select(v => v.Codigo)
                .FirstOrDefault();

            if (ultimoCodigo == null)
            {
                return "VEI001";
            }

            // Lógica para gerar próximo código
            return "VEI" + (int.Parse(ultimoCodigo[3..]) + 1).ToString("D3");
        }

        #region Endpoints Específicos

        [HttpPost]
        public async Task<IActionResult> AdicionarDocumento(long id, IFormFile arquivo, string descricao)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            var documento = new VeiculoDocumento
            {
                IdVeiculo = id,
                NomeArquivo = arquivo.FileName,
                Observacoes = descricao,
                DataUpload = DateTime.UtcNow
            };

            // Salvar arquivo e documento
            //SalvarArquivo(arquivo, documento);
            _context.VeiculoDocumentos.Add(documento);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Documento adicionado com sucesso!" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoverDocumento(int documentoId)
        {
            var documento = await _context.VeiculoDocumentos.FindAsync(documentoId);
            if (documento == null)
            {
                return NotFound();
            }

            _context.VeiculoDocumentos.Remove(documento);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Documento removido com sucesso!" });
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarFoto(long id, IFormFile foto, string descricao)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            var fotoEntidade = new VeiculoFoto
            {
                IdVeiculo = id,
                NomeArquivo = foto.FileName,
                Descricao = descricao,
                DataUpload = DateTime.UtcNow
            };

            //SalvarFoto(foto, fotoEntidade);
            _context.VeiculoFotos.Add(fotoEntidade);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Foto adicionada com sucesso!" });
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarDespesa(long id, DespesaCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var despesa = new Despesa
                {
                    IdVeiculo = id,
                    IdFornecedor = model.IdFornecedor,
                    Descricao = model.Descricao,
                    Valor = model.Valor,
                    DataDespesa = model.DataDespesa,
                    Status = EnumStatusDespesa.Pendente
                };

                _context.Despesas.Add(despesa);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Despesa adicionada com sucesso!" });
            }

            return Json(new { success = false, message = "Dados inválidos" });
        }

        [HttpPost]
        public async Task<IActionResult> Reservar(long id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo != null && veiculo.Situacao == EnumSituacaoVeiculo.Estoque)
            {
                veiculo.Situacao = EnumSituacaoVeiculo.Reservado;
                veiculo.DataAlteracao = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Veículo reservado com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível reservar o veículo!";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Liberar(long id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo != null && veiculo.Situacao == EnumSituacaoVeiculo.Reservado)
            {
                veiculo.Situacao = EnumSituacaoVeiculo.Estoque;
                veiculo.DataAlteracao = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Reserva do veículo liberada com sucesso!";
            }
            else
            {
                TempData["ErrorMessage"] = "Não foi possível liberar a reserva do veículo!";
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion ENDPOINTS ESPECÍFICOS
    }
}