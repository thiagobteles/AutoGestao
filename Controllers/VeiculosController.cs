using AutoGestao.Data;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Veiculo;
using AutoGestao.Models;
using AutoGestao.Extensions;
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
                .Include(v => v.Marca)
                .Include(v => v.Modelo)
                .Include(v => v.Cor)
                .Include(v => v.Proprietario)
                .AsQueryable();
        }

        protected override StandardGridViewModel ConfigureGrid()
        {
            return new StandardGridViewModel
            {
                Title = "Veículos",
                SubTitle = "Gerencie o estoque de veículos",
                EntityName = "Veiculos",
                ControllerName = "Veiculos",

                // Configuração das colunas
                Columns =
                [
                    new() { Name = nameof(Veiculo.Id), DisplayName = "Cód", Type = GridColumnType.Text, Sortable = true, Width = "70px"},
                    new() { Name = nameof(Veiculo.Codigo), DisplayName = "Código", Sortable = true, Width = "100px" },
                    new() { Name = "MarcaModelo", DisplayName = "Marca/Modelo", Sortable = false, CustomRender = RenderMarcaModelo },
                    new() { Name = nameof(Veiculo.AnoFabricacao), DisplayName = "Ano", Type = GridColumnType.Number, Sortable = true, Width = "80px" },
                    new() { Name = nameof(Veiculo.Placa), DisplayName = "Placa", Sortable = true, Width = "100px" },
                    new() { Name = nameof(Veiculo.KmSaida), DisplayName = "KM", Type = GridColumnType.Number, Sortable = true, Width = "100px" },
                    new() { Name = nameof(Veiculo.PrecoVenda), DisplayName = "Preço", Type = GridColumnType.Currency, Sortable = true, Width = "120px" },
                    new() { Name = nameof(Veiculo.Situacao), DisplayName = "Situação", Type = GridColumnType.Badge, Sortable = true, Width = "110px" },
                    new() { Name = nameof(Veiculo.StatusVeiculo), DisplayName = "Status", Type = GridColumnType.Badge, Sortable = true, Width = "110px" },
                    new() { Name = "Actions", DisplayName = "Ações", Type = GridColumnType.Actions, Sortable = false, Width = "150px" }
                ],

                // Configuração dos filtros
                Filters =
                [
                    new()
                    {
                        Name = "search",
                        DisplayName = "Busca Geral",
                        Type = GridFilterType.Text,
                        Placeholder = "Código, Placa, Marca, Modelo..."
                    },
                    new()
                    {
                        Name = "situacao",
                        DisplayName = "Situação",
                        Type = GridFilterType.Select,
                        Options = GetSituacaoOptions()
                    },
                    new()
                    {
                        Name = "marca",
                        DisplayName = "Marca",
                        Type = GridFilterType.Select,
                        Options = GetMarcaOptions()
                    },
                    new()
                    {
                        Name = "ano",
                        DisplayName = "Ano",
                        Type = GridFilterType.Number,
                        Placeholder = "Ex: 2023"
                    },
                    new()
                    {
                        Name = "combustivel",
                        DisplayName = "Combustível",
                        Type = GridFilterType.Select,
                        Options = GetCombustivelOptions()
                    },
                    new()
                    {
                        Name = "preco_min",
                        DisplayName = "Preço Mín",
                        Type = GridFilterType.Number,
                        Placeholder = "R$ 0,00"
                    },
                    new()
                    {
                        Name = "preco_max",
                        DisplayName = "Preço Máx",
                        Type = GridFilterType.Number,
                        Placeholder = "R$ 999.999,99"
                    },
                    new()
                    {
                        Name = "periodo_cadastro",
                        DisplayName = "Período de Cadastro",
                        Type = GridFilterType.DateRange
                    }
                ],

                // Ações do cabeçalho
                HeaderActions =
                [
                    new()
                    {
                        Name = "Create",
                        DisplayName = "Novo Veículo",
                        Icon = "fas fa-plus",
                        CssClass = "btn-new",
                        Url = Url.Action("Create", "Veiculos")
                    },
                    new()
                    {
                        Name = "Import",
                        DisplayName = "Importar",
                        Icon = "fas fa-upload",
                        CssClass = "btn-modern btn-outline-modern",
                        Url = Url.Action("Import", "Veiculos")
                    },
                    new()
                    {
                        Name = "Export",
                        DisplayName = "Exportar",
                        Icon = "fas fa-download",
                        CssClass = "btn-modern btn-outline-modern",
                        Url = Url.Action("Export", "Veiculos")
                    }
                ],

                // Ações das linhas
                RowActions =
                [
                    new()
                    {
                        Name = "Details",
                        DisplayName = "Visualizar",
                        Icon = "fas fa-eye",
                        Url = "/Veiculos/Details/{id}"
                    },
                    new()
                    {
                        Name = "Edit",
                        DisplayName = "Editar",
                        Icon = "fas fa-edit",
                        Url = "/Veiculos/Edit/{id}",
                        ShowCondition = (x) => ((Veiculo)x).Situacao != EnumSituacaoVeiculo.Vendido
                    },
                    new()
                    {
                        Name = "Photos",
                        DisplayName = "Fotos",
                        Icon = "fas fa-camera",
                        Url = "/Veiculos/Photos/{id}"
                    },
                    new()
                    {
                        Name = "Documents",
                        DisplayName = "Documentos",
                        Icon = "fas fa-file-alt",
                        Url = "/Veiculos/Documents/{id}"
                    },
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
                ]
            };
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
                            query = query.Where(v =>
                                v.Codigo.Contains(searchTerm) ||
                                v.Placa.Contains(searchTerm) ||
                                (v.Marca != null && v.Marca.Descricao.Contains(searchTerm)) ||
                                (v.Modelo != null && v.Modelo.Descricao.Contains(searchTerm)) ||
                                (v.Chassi != null && v.Chassi.Contains(searchTerm)) ||
                                (v.Renavam != null && v.Renavam.Contains(searchTerm)));
                        }
                        break;

                    case "situacao":
                        query = ApplyEnumFilter<EnumSituacaoVeiculo>(query, filters, filter.Key, v => v.Situacao);
                        break;

                    case "status":
                        query = ApplyEnumFilter<EnumStatusVeiculo>(query, filters, filter.Key, v => v.StatusVeiculo);
                        break;

                    case "combustivel":
                        query = ApplyEnumFilter<EnumCombustivelVeiculo>(query, filters, filter.Key, v => v.Combustivel);
                        break;

                    case "marca":
                        if (int.TryParse(filter.Value.ToString(), out int marcaId))
                        {
                            query = query.Where(v => v.VeiculoMarcaId == marcaId);
                        }
                        break;

                    case "modelo":
                        if (int.TryParse(filter.Value.ToString(), out int modeloId))
                        {
                            query = query.Where(v => v.VeiculoMarcaModeloId == modeloId);
                        }
                        break;

                    case "ano":
                        if (int.TryParse(filter.Value.ToString(), out int ano))
                        {
                            query = query.Where(v => v.AnoFabricacao == ano || v.AnoModelo == ano);
                        }
                        break;

                    case "preco_min":
                        if (decimal.TryParse(filter.Value.ToString(), out decimal precoMin))
                        {
                            query = query.Where(v => v.PrecoVenda >= precoMin);
                        }
                        break;

                    case "preco_max":
                        if (decimal.TryParse(filter.Value.ToString(), out decimal precoMax))
                        {
                            query = query.Where(v => v.PrecoVenda <= precoMax);
                        }
                        break;

                    case "km_min":
                        if (int.TryParse(filter.Value.ToString(), out int kmMin))
                        {
                            query = query.Where(v => v.KmSaida >= kmMin);
                        }
                        break;

                    case "km_max":
                        if (int.TryParse(filter.Value.ToString(), out int kmMax))
                        {
                            query = query.Where(v => v.KmSaida <= kmMax);
                        }
                        break;
                }
            }

            // Aplicar filtro de período usando o helper
            query = ApplyDateRangeFilter(query, filters, "periodo_cadastro", v => v.DataCadastro);

            return query;
        }

        protected override IQueryable<Veiculo> ApplySort(IQueryable<Veiculo> query, string orderBy, string orderDirection)
        {
            return orderBy?.ToLower() switch
            {
                "id" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.Id)
                    : query.OrderBy(v => v.Id),
                "codigo" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.Codigo)
                    : query.OrderBy(v => v.Codigo),
                "placa" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.Placa)
                    : query.OrderBy(v => v.Placa),
                "anofabricacao" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.AnoFabricacao)
                    : query.OrderBy(v => v.AnoFabricacao),
                "kmsaida" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.KmSaida)
                    : query.OrderBy(v => v.KmSaida),
                "precovenda" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.PrecoVenda)
                    : query.OrderBy(v => v.PrecoVenda),
                "situacao" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.Situacao)
                    : query.OrderBy(v => v.Situacao),
                "statusveiculo" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.StatusVeiculo)
                    : query.OrderBy(v => v.StatusVeiculo),
                "datacadastro" => orderDirection == "desc"
                    ? query.OrderByDescending(v => v.DataCadastro)
                    : query.OrderBy(v => v.DataCadastro),
                _ => query.OrderByDescending(v => v.DataCadastro) // Default: Últimos cadastros
            };
        }

        #region Métodos Auxiliares para Filtros

        private List<SelectListItem> GetSituacaoOptions()
        {
            var options = new List<SelectListItem>
            {
                new() { Value = "", Text = "Todas as Situações", Selected = true }
            };

            var enumDictionary = EnumExtension.GetEnumDictionary<EnumSituacaoVeiculo>();
            
            foreach (var item in enumDictionary.Where(x => x.Key != 0))
            {
                options.Add(new SelectListItem
                {
                    Value = ((EnumSituacaoVeiculo)item.Key).ToString(),
                    Text = $"{GetSituacaoIcon((EnumSituacaoVeiculo)item.Key)} {item.Value}"
                });
            }

            return options;
        }

        private List<SelectListItem> GetMarcaOptions()
        {
            var options = new List<SelectListItem>
            {
                new() { Value = "", Text = "Todas as Marcas", Selected = true }
            };

            var marcas = _context.VeiculoMarcas
                .OrderBy(m => m.Descricao)
                .Select(m => new { m.Id, m.Descricao })
                .ToList();

            foreach (var marca in marcas)
            {
                options.Add(new SelectListItem
                {
                    Value = marca.Id.ToString(),
                    Text = marca.Descricao
                });
            }

            return options;
        }

        private List<SelectListItem> GetCombustivelOptions()
        {
            var options = new List<SelectListItem>
            {
                new() { Value = "", Text = "Todos os Combustíveis", Selected = true }
            };

            var enumDictionary = EnumExtension.GetEnumDictionary<EnumCombustivelVeiculo>();
            
            foreach (var item in enumDictionary.Where(x => x.Key != 0))
            {
                options.Add(new SelectListItem
                {
                    Value = ((EnumCombustivelVeiculo)item.Key).ToString(),
                    Text = $"{GetCombustivelIcon((EnumCombustivelVeiculo)item.Key)} {item.Value}"
                });
            }

            return options;
        }

        private string GetSituacaoIcon(EnumSituacaoVeiculo situacao)
        {
            return situacao switch
            {
                EnumSituacaoVeiculo.Estoque => "📦",
                EnumSituacaoVeiculo.Vendido => "✅",
                EnumSituacaoVeiculo.Transferido => "🔄",
                _ => "❓"
            };
        }

        private string GetCombustivelIcon(EnumCombustivelVeiculo combustivel)
        {
            return combustivel switch
            {
                EnumCombustivelVeiculo.Gasolina => "⛽",
                EnumCombustivelVeiculo.Etanol => "🌽",
                EnumCombustivelVeiculo.Flex => "🔀",
                EnumCombustivelVeiculo.Diesel => "🚛",
                EnumCombustivelVeiculo.Eletrico => "🔋",
                EnumCombustivelVeiculo.Hibrido => "🔋⛽",
                _ => "❓"
            };
        }

        private string RenderMarcaModelo(object item)
        {
            var veiculo = (Veiculo)item;
            var marca = veiculo.Marca?.Descricao ?? "N/A";
            var modelo = veiculo.Modelo?.Descricao ?? "N/A";
            
            return $@"
                <div class=""vehicle-info"">
                    <div class=""fw-semibold"">{marca}</div>
                    <div class=""text-muted small"">{modelo}</div>
                </div>";
        }

        #endregion

        #region Ações Específicas

        [HttpPost]
        public async Task<IActionResult> Reserve(int id)
        {
            var veiculo = await _context.Veiculos.FindAsync(id);
            if (veiculo != null && veiculo.Situacao == EnumSituacaoVeiculo.Estoque)
            {
                veiculo.Situacao = EnumSituacaoVeiculo.Reservado;
                veiculo.DataAlteracao = DateTime.Now;
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
                veiculo.DataAlteracao = DateTime.Now;
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
                .Include(v => v.Marca)
                .Include(v => v.Modelo)
                .Include(v => v.Cor)
                .Include(v => v.Proprietario)
                .Include(v => v.Fotos)
                .Include(v => v.Documentos)
                .Include(v => v.Vendas)
                .Include(v => v.Despesas)
                .FirstOrDefaultAsync(m => m.Id == id);

            return veiculo == null ? NotFound() : View(veiculo);
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
                    veiculo.DataCadastro = DateTime.Now;
                    veiculo.DataAlteracao = DateTime.Now;
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
                        veiculo.DataAlteracao = DateTime.Now;
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

        [HttpGet]
        public async Task<IActionResult> Export()
        {
            try
            {
                var veiculos = await _context.Veiculos
                    .Include(v => v.Marca)
                    .Include(v => v.Modelo)
                    .Include(v => v.Cor)
                    .OrderBy(v => v.Codigo)
                    .ToListAsync();

                var csv = new System.Text.StringBuilder();
                csv.AppendLine("ID,Código,Marca,Modelo,Ano,Placa,KM,Preço Compra,Preço Venda,Situação,Status,Data Cadastro");

                foreach (var veiculo in veiculos)
                {
                    csv.AppendLine($"{veiculo.Id}," +
                                  $"{veiculo.Codigo}," +
                                  $"\"{veiculo.Marca?.Descricao}\"," +
                                  $"\"{veiculo.Modelo?.Descricao}\"," +
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
                var fileName = $"veiculos_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                
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
    }
}