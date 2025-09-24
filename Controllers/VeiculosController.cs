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
                        Placeholder = "Código, Placa, Marca, Modelo..."
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
                        if (!string.IsNullOrEmpty(searchTerm.ToLower()))
                        {
                            query = ApplyTextFilter(query, searchTerm,
                                c => c.Codigo.ToLower(),
                                c => c.Placa.ToLower(),
                                c => c.VeiculoMarca.Descricao.ToLower(),
                                c => c.VeiculoMarcaModelo.Descricao.ToLower(),
                                c => c.Chassi.ToLower(),
                                c => c.Renavam.ToLower());
                        }
                        break;

                    case "situacao":
                        query = ApplyEnumFilter(query, filters, filter.Key, v => v.Situacao);
                        break;
                }
            }

            // Aplicar filtro de período usando o helper
            query = ApplyDateRangeFilter(query, filters, "periodo_cadastro", v => v.DataCadastro);
            return query;
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

        #region Ações Específicas

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

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos
                .Include(v => v.VeiculoMarca)
                .Include(v => v.VeiculoMarcaModelo)
                .Include(v => v.VeiculoCor)
                .Include(v => v.Proprietario)
                .Include(v => v.Fotos)
                .Include(v => v.Documentos)
                .Include(v => v.Vendas)
                .Include(v => v.Despesas)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (veiculo == null)
            {
                return NotFound();
            }

            // Adicionar histórico de auditoria
            await this.AddAuditHistoryToViewBag(_context, veiculo);

            var viewModel = BuildFormViewModel(veiculo, "Details");
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Veiculo veiculo)
        {
            if (ModelState.IsValid)
            {
                // Validações específicas
                var existeCodigo = await _context.Veiculos.AnyAsync(v => v.Codigo == veiculo.Codigo);
                if (existeCodigo)
                {
                    ModelState.AddModelError("Codigo", "Já existe um veículo com este código");
                }

                var existePlaca = await _context.Veiculos.AnyAsync(v => v.Placa == veiculo.Placa);
                if (existePlaca)
                {
                    ModelState.AddModelError("Placa", "Já existe um veículo com esta placa");
                }

                if (!string.IsNullOrEmpty(veiculo.Chassi))
                {
                    var existeChassi = await _context.Veiculos.AnyAsync(v => v.Chassi == veiculo.Chassi);
                    if (existeChassi)
                    {
                        ModelState.AddModelError("Chassi", "Já existe um veículo com este chassi");
                    }
                }

                if (ModelState.IsValid)
                {
                    veiculo.DataCadastro = DateTime.UtcNow;
                    veiculo.DataAlteracao = DateTime.UtcNow;
                    _context.Add(veiculo);
                    await _context.SaveChangesAsync();
                    
                    TempData["SuccessMessage"] = "Veículo cadastrado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Marcas = GetMarcasSelectList();
            ViewBag.Cores = GetCoresSelectList();
            ViewBag.Proprietarios = GetProprietariosSelectList();

            return View(veiculo);
        }

        public IActionResult Create()
        {
            ViewBag.Marcas = GetMarcasSelectList();
            ViewBag.Cores = GetCoresSelectList();
            ViewBag.Proprietarios = GetProprietariosSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validações específicas (exceto o próprio registro)
                    var existeCodigo = await _context.Veiculos.AnyAsync(v => v.Codigo == veiculo.Codigo && v.Id != veiculo.Id);
                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Já existe outro veículo com este código");
                    }

                    var existePlaca = await _context.Veiculos.AnyAsync(v => v.Placa == veiculo.Placa && v.Id != veiculo.Id);
                    if (existePlaca)
                    {
                        ModelState.AddModelError("Placa", "Já existe outro veículo com esta placa");
                    }

                    if (!string.IsNullOrEmpty(veiculo.Chassi))
                    {
                        var existeChassi = await _context.Veiculos.AnyAsync(v => v.Chassi == veiculo.Chassi && v.Id != veiculo.Id);
                        if (existeChassi)
                        {
                            ModelState.AddModelError("Chassi", "Já existe outro veículo com este chassi");
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        veiculo.DataAlteracao = DateTime.UtcNow;
                        _context.Update(veiculo);
                        await _context.SaveChangesAsync();
                        
                        TempData["SuccessMessage"] = "Veículo atualizado com sucesso!";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculoExists(veiculo.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }

            ViewBag.Marcas = GetMarcasSelectList();
            ViewBag.Cores = GetCoresSelectList();
            ViewBag.Proprietarios = GetProprietariosSelectList();
            return View(veiculo);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            ViewBag.Marcas = GetMarcasSelectList();
            ViewBag.Cores = GetCoresSelectList();
            ViewBag.Proprietarios = GetProprietariosSelectList();
            return View(veiculo);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, Veiculo veiculo)
        {
            if (id != veiculo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Remove(veiculo);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Veículo deletado com sucesso!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VeiculoExists(veiculo.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(veiculo);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo == null)
            {
                return NotFound();
            }

            _context.Remove(veiculo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Veículo deletado com sucesso!";
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

        #endregion Ações Específicas

        #region Métodos Auxiliares

        private SelectList GetMarcasSelectList()
        {
            var marcas = _context.VeiculoMarcas
                .OrderBy(m => m.Descricao)
                .Select(m => new { m.Id, m.Descricao })
                .ToList();
                
            return new SelectList(marcas, "Id", "Descricao");
        }

        private SelectList GetCoresSelectList()
        {
            var cores = _context.VeiculoCores
                .OrderBy(c => c.Descricao)
                .Select(c => new { c.Id, c.Descricao })
                .ToList();
                
            return new SelectList(cores, "Id", "Descricao");
        }

        private SelectList GetProprietariosSelectList()
        {
            var proprietarios = _context.Clientes
                .Where(c => c.Ativo)
                .OrderBy(c => c.Nome)
                .Select(c => new { c.Id, c.Nome })
                .ToList();
                
            return new SelectList(proprietarios, "Id", "Nome");
        }

        private bool VeiculoExists(int id)
        {
            return _context.Veiculos.Any(e => e.Id == id);
        }

        #endregion  Métodos Auxiliares

        // ================================================================================================
        // NOVOS MÉTODOS PARA FORMULÁRIOS DINÂMICOS
        // ================================================================================================

        protected override List<SelectListItem> GetSelectOptions(string propertyName)
        {
            return propertyName switch
            {
                nameof(Veiculo.VeiculoMarcaId) => _context.VeiculoMarcas
                    .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Descricao })
                    .ToList(),
                nameof(Veiculo.VeiculoMarcaModeloId) => _context.VeiculoMarcaModelos
                    .Select(m => new SelectListItem { Value = m.Id.ToString(), Text = m.Descricao })
                    .ToList(),
                nameof(Veiculo.ProprietarioId) => _context.Clientes
                    .Where(c => c.Ativo)
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Nome })
                    .ToList(),
                nameof(Veiculo.Situacao) => EnumExtension.GetSelectListItems<EnumSituacaoVeiculo>(true),
                _ => base.GetSelectOptions(propertyName)
            };
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

            ValidateVeiculo(entity);
            return base.BeforeCreate(entity);
        }

        protected override Task BeforeUpdate(Veiculo entity)
        {
            entity.DataAlteracao = DateTime.UtcNow;
            ValidateVeiculo(entity);
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

        // ================================================================================================
        // ACTIONS ESPECÍFICAS PARA CRUD DAS ABAS
        // ================================================================================================

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

        #region Métodos privados

        private void ValidateVeiculo(Veiculo entity)
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

    // ================================================================================================
    // VIEWMODELS AUXILIARES
    // ================================================================================================

    public class VeiculoResumoViewModel
    {
        public Veiculo Veiculo { get; set; }
        public decimal TotalDespesas { get; set; }
        public int QtdDocumentos { get; set; }
        public int QtdFotos { get; set; }
        public List<Despesa> UltimasDespesas { get; set; } = [];
    }

    public class DespesaCreateViewModel
    {
        public int FornecedorId { get; set; }
        public string Descricao { get; set; } = "";
        public decimal Valor { get; set; }
        public DateTime DataDespesa { get; set; } = DateTime.Today;
    }
}