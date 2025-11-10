using FGT.Atributes;
using FGT.Data;
using FGT.Entidades.Base;
using FGT.Entidades.Fiscal;
using FGT.Enumerador.Gerais;
using FGT.Interfaces;
using FGT.Models;
using FGT.Services.Interface;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace FGT.Entidades.Processing
{
    [FormConfig(Title = "Importação de Negociações Fiscais", Subtitle = "Importe planilhas Excel com dados de negociações", Icon = "fas fa-file-excel")]
    public class ImportacaoNegociacaoFiscal : BaseEntidade, IProcessingEntity<ImportacaoNegociacaoFiscalResult>
    {
        [FormField(Name = "Arquivo Excel", Order = 10, Section = "Arquivo", Icon = "fas fa-file-upload", Type = EnumFieldType.File, Required = true, AllowedExtensions = "xls,xlsx", MaxSizeMB = 10, Placeholder = "Selecione um arquivo .xls ou .xlsx")]
        [Required(ErrorMessage = "O arquivo Excel é obrigatório")]
        public string? ArquivoExcel { get; set; }

        [FormField(Name = "Descrição", Order = 20, Section = "Arquivo", Icon = "fas fa-sticky-note", Type = EnumFieldType.TextArea, Placeholder = "Descrição opcional da importação...")]
        public string? Descricao { get; set; }

        public async Task<ProcessingResult<ImportacaoNegociacaoFiscalResult>> ProcessAsync(ApplicationDbContext context, IFileStorageService fileStorageService, long userId, long empresaId)
        {
            var resultado = new ImportacaoNegociacaoFiscalResult();

            try
            {
                if (string.IsNullOrEmpty(ArquivoExcel))
                {
                    return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                    {
                        Success = false,
                        Message = "Nenhum arquivo foi enviado.",
                        Data = resultado
                    };
                }

                // Baixar arquivo do storage
                using var fileStream = await fileStorageService.DownloadFileAsync(ArquivoExcel,"ImportacaoNegociacaoFiscal", empresaId);
                if (fileStream == null || fileStream.Length == 0)
                {
                    resultado.Erros.Add("Arquivo não encontrado no servidor");
                    return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                    {
                        Success = false,
                        Message = "Arquivo não encontrado no servidor.",
                        Data = resultado
                    };
                }

                // Criar pacote Excel
                using var package = new ExcelPackage(fileStream);
                var worksheet = package.Workbook.Worksheets[0];

                if (worksheet == null)
                {
                    resultado.Erros.Add("A planilha não contém nenhuma aba");
                    return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                    {
                        Success = false,
                        Message = "Planilha vazia ou inválida.",
                        Data = resultado
                    };
                }

                var rowCount = worksheet.Dimension?.Rows ?? 0;
                if (rowCount <= 2)
                {
                    resultado.Avisos.Add("A planilha não contém linhas de dados (apenas cabeçalho)");
                    return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                    {
                        Success = false,
                        Message = "Planilha sem dados para importar.",
                        Data = resultado
                    };
                }

                resultado.TotalLinhas = rowCount - 2;

                var negociacoes = new List<NegociacaoFiscal>();

                // Processar linhas (começando da linha 3)
                for (int row = 3; row <= rowCount; row++)
                {
                    try
                    {
                        var negociacao = new NegociacaoFiscal
                        {
                            MesAnoRequerimento = GetCellValue(worksheet, row, 1)?.ToString() ?? "",
                            UFOptante = GetCellValue(worksheet, row, 2)?.ToString() ?? "",
                            CpfCnpjOptante = GetCellValue(worksheet, row, 3)?.ToString() ?? "",
                            NomeOptante = GetCellValue(worksheet, row, 4)?.ToString() ?? "",
                            NumeroContaNegociacao = GetCellValue(worksheet, row, 5)?.ToString() ?? "",
                            TipoNegociacao = GetCellValue(worksheet, row, 6)?.ToString(),
                            ModalidadeNegociacao = GetCellValue(worksheet, row, 7)?.ToString(),
                            SituacaoNegociacao = GetCellValue(worksheet, row, 8)?.ToString(),
                            QtdeParcelasConcedidas = ParseInt(GetCellValue(worksheet, row, 9)),
                            QtdeParcelasAtraso = ParseInt(GetCellValue(worksheet, row, 10)),
                            ValorConsolidado = ParseDecimal(GetCellValue(worksheet, row, 11)),
                            ValorPrincipal = ParseDecimal(GetCellValue(worksheet, row, 12)),
                            ValorMulta = ParseDecimal(GetCellValue(worksheet, row, 13)),
                            ValorJuros = ParseDecimal(GetCellValue(worksheet, row, 14)),
                            ValorEncargoLegal = ParseDecimal(GetCellValue(worksheet, row, 15)),
                            CriadoPorUsuarioId = userId
                        };

                        // Validações básicas
                        if (string.IsNullOrWhiteSpace(negociacao.MesAnoRequerimento) ||
                            string.IsNullOrWhiteSpace(negociacao.CpfCnpjOptante) ||
                            string.IsNullOrWhiteSpace(negociacao.NomeOptante) ||
                            string.IsNullOrWhiteSpace(negociacao.NumeroContaNegociacao))
                        {
                            resultado.Erros.Add($"Linha {row}: Campos obrigatórios não preenchidos");
                            resultado.LinhasComErro++;
                            continue;
                        }

                        negociacoes.Add(negociacao);
                        resultado.LinhasImportadas++;
                    }
                    catch (Exception ex)
                    {
                        resultado.Erros.Add($"Linha {row}: {ex.Message}");
                        resultado.LinhasComErro++;
                    }
                }

                // Salvar no banco
                if (negociacoes.Count != 0)
                {
                    await context.Set<NegociacaoFiscal>().AddRangeAsync(negociacoes);
                    await context.SaveChangesAsync();
                }

                resultado.Sucesso = resultado.LinhasImportadas > 0;
                resultado.Mensagem = resultado.Sucesso
                    ? $"Importação concluída! {resultado.LinhasImportadas} de {resultado.TotalLinhas} linhas importadas."
                    : "Nenhuma linha foi importada. Verifique os erros.";

                return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                {
                    Success = resultado.Sucesso,
                    Message = resultado.Mensagem,
                    Data = resultado
                };
            }
            catch (Exception ex)
            {
                resultado.Erros.Add(ex.ToString());
                return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                {
                    Success = false,
                    Message = $"Erro ao processar arquivo: {ex.Message}",
                    Data = resultado
                };
            }
        }

        private static object? GetCellValue(ExcelWorksheet worksheet, int row, int col)
        {
            try 
            {
                return worksheet.Cells[row, col].Value;
            }
            catch
            {
                return null;
            }
        }

        private static int? ParseInt(object? value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is int intValue)
            {
                return intValue;
            }

            if (value is double doubleValue)
            {
                return (int)doubleValue;
            }

            if (int.TryParse(value.ToString(), out var result))
            {
                return result;
            }

            return null;
        }

        private static decimal? ParseDecimal(object? value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is decimal decimalValue)
            {
                return decimalValue;
            }

            if (value is double doubleValue)
            {
                return (decimal)doubleValue;
            }

            var stringValue = value.ToString()?.Replace("R$", "").Trim();
            if (decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.GetCultureInfo("pt-BR"), out var result))
            {
                return result;
            }

            return null;
        }
    }
}