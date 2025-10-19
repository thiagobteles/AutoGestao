using AutoGestao.Controllers.Base;
using AutoGestao.Data;
using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador.Veiculo;
using AutoGestao.Services;
using AutoGestao.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Controllers
{
    public class VeiculoController(
        ApplicationDbContext context,
        IFileStorageService fileStorageService,
        ILogger<AutoGridController<Veiculo>> logger) : UltraGenericController<Veiculo>(context, fileStorageService, logger)
    {
        /// <summary>
        /// Exemplo de customização: validação específica para veículos
        /// </summary>
        protected override async Task ApplyCustomValidationsAsync(Veiculo entity, string action)
        {
            // Validar placa única
            var placaExistente = await _context.Veiculos
                .AnyAsync(v => v.Id != entity.Id && v.Placa == entity.Placa);
            
            if (placaExistente)
            {
                ModelState.AddModelError(nameof(entity.Placa), "Placa já cadastrada");
            }

            // Validar anos
            if (entity.AnoModelo < entity.AnoFabricacao)
            {
                ModelState.AddModelError(nameof(entity.AnoModelo), 
                    "Ano do modelo não pode ser menor que ano de fabricação");
            }
        }

        /// <summary>
        /// Exemplo de customização: valores padrão
        /// </summary>
        protected override async Task SetDefaultValues(Veiculo entity)
        {
            if (string.IsNullOrEmpty(entity.Codigo))
            {
                entity.Codigo = await GerarProximoCodigoAsync();
            }
        }

        /// <summary>
        /// Exemplo de customização: ação customizada
        /// </summary>
        protected override async Task<object> ExecuteCustomActionAsync(string actionName, long[] ids, Dictionary<string, object> parameters)
        {
            return actionName switch
            {
                "MarcarComoVendido" => await MarcarVeiculosComoVendidosAsync(ids),
                "GerarRelatorioCustom" => await GerarRelatorioCustomAsync(ids, parameters),
                _ => await base.ExecuteCustomActionAsync(actionName, ids, parameters)
            };
        }

        #region Métodos Auxiliares Customizados

        private async Task<string> GerarProximoCodigoAsync()
        {
            var ultimoCodigo = await _context.Veiculos
                .OrderByDescending(v => v.Id)
                .Select(v => v.Codigo)
                .FirstOrDefaultAsync();

            if (ultimoCodigo == null)
            {
                return "VEI001";
            }

            var numero = int.Parse(ultimoCodigo[3..]) + 1;
            return $"VEI{numero:D3}";
        }

        private async Task<object> MarcarVeiculosComoVendidosAsync(long[] ids)
        {
            var veiculos = await _context.Veiculos
                .Where(v => ids.Contains(v.Id))
                .ToListAsync();

            foreach (var veiculo in veiculos)
            {
                veiculo.Situacao = EnumSituacaoVeiculo.Vendido;
                veiculo.DataAlteracao = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            return new 
            { 
                success = true, 
                message = $"{veiculos.Count} veículo(s) marcado(s) como vendido(s)",
                affectedRecords = veiculos.Count
            };
        }

        private static async Task<object> GerarRelatorioCustomAsync(long[] ids, Dictionary<string, object> parameters)
        {
            // Implementar geração de relatório customizado
            return new { success = true, message = "Relatório gerado com sucesso!" };
        }

        #endregion
    }

    public class ClienteController(
        ApplicationDbContext context,
        IFileStorageService fileStorageService,
        ILogger<AutoGridController<Cliente>> logger) : UltraGenericController<Cliente>(context, fileStorageService, logger)
    {
    }

    public class FornecedorController(
        ApplicationDbContext context,
        IFileStorageService fileStorageService,
        ILogger<AutoGridController<Fornecedor>> logger) : UltraGenericController<Fornecedor>(context, fileStorageService, logger)
    {
    }
}