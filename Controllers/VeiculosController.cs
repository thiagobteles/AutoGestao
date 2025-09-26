using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Enumerador.Veiculo;
using AutoGestao.Extensions;
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
                .Include(v => v.Proprietario)
                .AsQueryable();
        }

        protected override StandardGridViewModel ConfigureGrid()
        {
            var retorno = new StandardGridViewModel("Veículos", "Gerencie o estoque de veículos", "Veiculos")
            {
                // Configuração dos filtros
                Filters =
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
                ],

                // Configuração das colunas
                Columns =
                [
                    new() { Name = nameof(Veiculo.Id), DisplayName = "Cód", Type = EnumGridColumnType.Text, Sortable = true, Width = "65px" },
                    new() { Name = "MarcaModelo", DisplayName = "Marca/Modelo", Sortable = false, Type = EnumGridColumnType.Custom, CustomRender = RenderMarcaModelo },
                    new() { Name = nameof(Veiculo.AnoFabricacao), DisplayName = "Ano", Type = EnumGridColumnType.Integer, Sortable = true},
                    new() { Name = nameof(Veiculo.Placa), DisplayName = "Placa", Sortable = true },
                    new() { Name = nameof(Veiculo.KmSaida), DisplayName = "KM", Type = EnumGridColumnType.Number, Sortable = true },
                    new() { Name = nameof(Veiculo.PrecoVenda), DisplayName = "Preço", Type = EnumGridColumnType.Currency, Sortable = true },
                    new() { Name = nameof(Veiculo.Situacao), DisplayName = "Situação", Type = EnumGridColumnType.Enumerador, EnumRender = EnumRenderType.IconDescription, Sortable = true },
                    new() { Name = nameof(Veiculo.StatusVeiculo), DisplayName = "Status", Type = EnumGridColumnType.Enumerador, EnumRender = EnumRenderType.Description, Sortable = true },
                    new() { Name = "Actions", DisplayName = "Ações", Type = EnumGridColumnType.Actions, Sortable = false, Width = "100px" }
                ],
            };

            retorno.RowActions.AddRange(
                [
                    new()
                    {
                        Name = "Sell",
                        DisplayName = "Vender",
                        Icon = "fas fa-handshake",
                        Url = "/Vendas/Create?veiculoId={id}",
                        ShowCondition = (x) => ((Veiculo)x).Situacao == EnumSituacaoVeiculo.Estoque
                    },
                    new()
                    {
                        Name = "Reserve",
                        DisplayName = "Reservar",
                        Icon = "fas fa-bookmark",
                        Url = "/Veiculos/Reserve/{id}",
                        ShowCondition = (x) => ((Veiculo)x).Situacao == EnumSituacaoVeiculo.Estoque
                    },
                    new()
                    {
                        Name = "Unreserve",
                        DisplayName = "Liberar",
                        Icon = "fas fa-bookmark-o",
                        Url = "/Veiculos/Unreserve/{id}",
                        ShowCondition = (x) => ((Veiculo)x).Situacao == EnumSituacaoVeiculo.Reservado
                    }
                ]);

            return retorno;
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

            // Aplicar filtro de período usando o helper
            //query = ApplyDateRangeFilter(query, filters, "periodo_cadastro", v => v.DataCadastro);
            return query;
        }

        protected override Task BeforeCreate(Veiculo entity)
        {
            entity.DataCadastro = DateTime.UtcNow;
            entity.Situacao = EnumSituacaoVeiculo.Estoque;

            // Gerar código se não informado
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

        protected override async Task<IActionResult> RenderCustomTab(Veiculo entity, FormTabViewModel tab)
        {
            return tab.TabId switch
            {
                "arquivos" => await RenderArquivosTab(entity),
                "midias" => await RenderMidiasTab(entity),
                "financeiro" => await RenderFinanceiroTab(entity),
                "resumo" => await RenderResumoTab(entity),
                _ => await base.RenderCustomTab(entity, tab)
            };
        }

        #region ENDPOINTS ESPECÍFICOS

        [HttpPost]
        public async Task<IActionResult> AdicionarDocumento(int veiculoId, IFormFile arquivo, string descricao)
        {
            var veiculo = await _context.Veiculos.FindAsync(veiculoId);
            if (veiculo == null)
            {
                return NotFound();
            }

            var documento = new VeiculoDocumento
            {
                VeiculoId = veiculoId,
                NomeArquivo = arquivo.FileName,
                Observacoes = descricao,
                DataUpload = DateTime.UtcNow
            };

            // Salvar arquivo e documento
            SalvarArquivo(arquivo, documento);
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
        public async Task<IActionResult> AdicionarFoto(int veiculoId, IFormFile foto, string descricao)
        {
            var veiculo = await _context.Veiculos.FindAsync(veiculoId);
            if (veiculo == null)
            {
                return NotFound();
            }

            var fotoEntidade = new VeiculoFoto
            {
                VeiculoId = veiculoId,
                NomeArquivo = foto.FileName,
                Descricao = descricao,
                DataUpload = DateTime.UtcNow
            };

            SalvarFoto(foto, fotoEntidade);
            _context.VeiculoFotos.Add(fotoEntidade);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Foto adicionada com sucesso!" });
        }

        [HttpPost]
        public async Task<IActionResult> AdicionarDespesa(int veiculoId, DespesaCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var despesa = new Despesa
                {
                    VeiculoId = veiculoId,
                    FornecedorId = model.FornecedorId,
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
        public async Task<IActionResult> Reserve(int id)
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
        public async Task<IActionResult> Unreserve(int id)
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

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            try
            {
                var veiculos = await _context.Veiculos
                    .Include(v => v.VeiculoMarca)
                    .Include(v => v.VeiculoMarcaModelo)
                    .Include(v => v.VeiculoCor)
                    .OrderBy(v => v.Codigo)
                    .ToListAsync();

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("ID,Código,Marca,Modelo,Ano,Placa,KM,Preço Compra,Preço Venda,Situação,Status,Data Cadastro");

                foreach (var veiculo in veiculos)
                {
                    csv.AppendLine($"{veiculo.Id}," +
                                  $"{veiculo.Codigo}," +
                                  $"\"{veiculo.VeiculoMarca?.Descricao}\"," +
                                  $"\"{veiculo.VeiculoMarcaModelo?.Descricao}\"," +
                                  $"{veiculo.AnoFabricacao}," +
                                  $"{veiculo.Placa}," +
                                  $"{veiculo.KmSaida}," +
                                  $"{veiculo.PrecoCompra:F2}," +
                                  $"{veiculo.PrecoVenda:F2}," +
                                  $"{veiculo.Situacao.GetDescription()}," +
                                  $"{veiculo.StatusVeiculo.GetDescription()}," +
                                  $"{veiculo.DataCadastro:dd/MM/yyyy}");
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
                var fileName = $"veiculos_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao exportar dados: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        #endregion ENDPOINTS ESPECÍFICOS

        #region Métodos privados

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
            return "VEI" + (int.Parse(ultimoCodigo.Substring(3)) + 1).ToString("D3");
        }

        private static string RenderMarcaModelo(object item)
        {
            var veiculo = (Veiculo)item;
            var marca = veiculo.VeiculoMarca?.Descricao ?? "N/A";
            var modelo = veiculo.VeiculoMarcaModelo?.Descricao ?? "N/A";

            return $@"
                <div class=""vehicle-info"">
                    <div class=""fw-semibold"">{marca}</div>
                    <div class=""text-muted small"">{modelo}</div>
                </div>";
        }

        private async Task<IActionResult> RenderArquivosTab(Veiculo veiculo)
        {
            var documentos = await _context.VeiculoDocumentos
                .Where(d => d.VeiculoId == veiculo.Id)
                .ToListAsync();

            return PartialView("_TabArquivos", documentos);
        }

        private async Task<IActionResult> RenderMidiasTab(Veiculo veiculo)
        {
            var fotos = await _context.VeiculoFotos
                .Where(f => f.VeiculoId == veiculo.Id)
                .ToListAsync();

            return PartialView("_TabMidias", fotos);
        }

        private async Task<IActionResult> RenderFinanceiroTab(Veiculo veiculo)
        {
            var despesas = await _context.Despesas
                .Where(d => d.VeiculoId == veiculo.Id)
                .Include(d => d.Fornecedor)
                .ToListAsync();

            return PartialView("_TabFinanceiro", despesas);
        }

        private async Task<IActionResult> RenderResumoTab(Veiculo veiculo)
        {
            var resumo = new VeiculoResumoViewModel
            {
                Veiculo = veiculo,
                TotalDespesas = await _context.Despesas
                    .Where(d => d.VeiculoId == veiculo.Id)
                    .SumAsync(d => d.Valor),
                QtdDocumentos = await _context.VeiculoDocumentos
                    .CountAsync(d => d.VeiculoId == veiculo.Id),
                QtdFotos = await _context.VeiculoFotos
                    .CountAsync(f => f.VeiculoId == veiculo.Id)
            };

            return PartialView("_TabResumo", resumo);
        }

        private void SalvarArquivo(IFormFile arquivo, VeiculoDocumento documento)
        {
            // Implementar lógica de salvamento do arquivo
            var caminho = Path.Combine("uploads", "documentos", $"{documento.Id}_{arquivo.FileName}");
            // ... lógica de salvamento
        }

        private void SalvarFoto(IFormFile foto, VeiculoFoto fotoEntidade)
        {
            // Implementar lógica de salvamento da foto
            var caminho = Path.Combine("uploads", "fotos", $"{fotoEntidade.Id}_{foto.FileName}");
            // ... lógica de salvamento
        }

        #endregion Métodos privados
    }
}