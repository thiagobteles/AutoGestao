using FGT.Atributes;
using FGT.Data;
using FGT.Entidades.Base;
using FGT.Enumerador.Gerais;
using FGT.Interfaces;
using FGT.Models;
using FGT.Services.Interface;
using FGT.Extensions;
using ExcelDataReader;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Data;
using System.Text;

namespace FGT.Entidades.Processamento
{
    [ProcessingForm(Title = "Importação de Negociações Fiscais", Description = "Importe planilhas Excel com dados de negociações fiscais",
        Icon = "fas fa-file-excel",
        SubmitButtonText = "Importar Planilha",
        SubmitButtonIcon = "fas fa-upload",
        RedirectOnSuccessUrl = "/NegociacaoFiscal/Index",
        AllowMultipleExecutions = false
    )]
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
                    resultado.Erros.Add("Nenhum arquivo foi enviado");
                    return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                    {
                        Success = false,
                        Message = "Nenhum arquivo foi enviado.",
                        Data = resultado,
                        Errors = resultado.Erros
                    };
                }

                // Registrar encoding provider para suportar codepages (necessário para ExcelDataReader)
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                // Baixar arquivo do storage
                using var fileStream = await fileStorageService.DownloadFileAsync(ArquivoExcel,"ImportacaoNegociacaoFiscal", empresaId);
                if (fileStream == null || fileStream.Length == 0)
                {
                    resultado.Erros.Add("Arquivo não encontrado no servidor");
                    return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                    {
                        Success = false,
                        Message = "Arquivo não encontrado no servidor.",
                        Data = resultado,
                        Errors = resultado.Erros
                    };
                }

                // Criar leitor Excel
                using var reader = ExcelReaderFactory.CreateReader(fileStream);

                // Configurar para ler como DataSet
                var dataSetConfig = new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = false // Não usar primeira linha como cabeçalho
                    }
                };

                using var dataSet = reader.AsDataSet(dataSetConfig);

                if (dataSet.Tables.Count == 0)
                {
                    resultado.Erros.Add("A planilha não contém nenhuma aba");
                    return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                    {
                        Success = false,
                        Message = "Planilha vazia ou inválida.",
                        Data = resultado,
                        Errors = resultado.Erros
                    };
                }

                var dataTable = dataSet.Tables[0];
                var rowCount = dataTable.Rows.Count;

                if (rowCount <= 2)
                {
                    resultado.Avisos.Add("A planilha não contém linhas de dados (apenas cabeçalho)");
                    return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                    {
                        Success = false,
                        Message = "Planilha sem dados para importar.",
                        Data = resultado,
                        Warnings = resultado.Avisos
                    };
                }

                resultado.TotalLinhas = rowCount - 2;

                var negociacoes = new List<NegociacaoFiscal>();

                // Variáveis para armazenar último valor válido (células mescladas)
                string? lastMesAno = null;
                string? lastUF = null;
                string? lastCpfCnpj = null;
                string? lastNome = null;
                string? lastConta = null;

                // Processar linhas (começando da linha 2 - índice 2, pois índice 0 e 1 são cabeçalhos)
                for (var rowIndex = 2; rowIndex < rowCount; rowIndex++)
                {
                    try
                    {
                        var row = dataTable.Rows[rowIndex];

                        // Obter valores com lógica de células mescladas (último valor válido)
                        var mesAno = SafeString(GetCellValue(row, 0), 7);
                        var uf = SafeString(GetCellValue(row, 1), 2);
                        var cpfCnpj = SafeString(GetCellValue(row, 2), 18);
                        var nome = SafeString(GetCellValue(row, 3), 200);
                        var conta = SafeString(GetCellValue(row, 4), 50);

                        // Atualizar último valor válido se não estiver vazio
                        if (!string.IsNullOrWhiteSpace(mesAno)) lastMesAno = mesAno;
                        if (!string.IsNullOrWhiteSpace(uf)) lastUF = uf;
                        if (!string.IsNullOrWhiteSpace(cpfCnpj)) lastCpfCnpj = cpfCnpj;
                        if (!string.IsNullOrWhiteSpace(nome)) lastNome = nome;
                        if (!string.IsNullOrWhiteSpace(conta)) lastConta = conta;

                        var negociacao = new NegociacaoFiscal
                        {
                            IdEmpresa = empresaId,  // ✅ CRÍTICO: Campo necessário para filtrar dados por empresa
                            CriadoPorUsuarioId = userId,
                            MesAnoRequerimento = lastMesAno ?? "",
                            UFOptante = lastUF ?? "",
                            CpfCnpjOptante = lastCpfCnpj ?? "",
                            NomeOptante = lastNome ?? "",
                            NumeroContaNegociacao = lastConta ?? "",
                            TipoNegociacao = SafeNullableString(GetCellValue(row, 5), 100),
                            ModalidadeNegociacao = SafeNullableString(GetCellValue(row, 6), 500),
                            SituacaoNegociacao = SafeNullableString(GetCellValue(row, 7), 100),
                            QtdeParcelasConcedidas = ParseInt(GetCellValue(row, 8)),
                            QtdeParcelasAtraso = ParseInt(GetCellValue(row, 9)),
                            ValorConsolidado = ParseDecimal(GetCellValue(row, 10)),
                            ValorPrincipal = ParseDecimal(GetCellValue(row, 11)),
                            ValorMulta = ParseDecimal(GetCellValue(row, 12)),
                            ValorJuros = ParseDecimal(GetCellValue(row, 13)),
                            ValorEncargoLegal = ParseDecimal(GetCellValue(row, 14))
                        };

                        // Validações básicas
                        if (string.IsNullOrWhiteSpace(negociacao.MesAnoRequerimento) ||
                            string.IsNullOrWhiteSpace(negociacao.CpfCnpjOptante) ||
                            string.IsNullOrWhiteSpace(negociacao.NomeOptante) ||
                            string.IsNullOrWhiteSpace(negociacao.NumeroContaNegociacao))
                        {
                            resultado.Erros.Add($"Linha {rowIndex + 1}: Campos obrigatórios não preenchidos (MesAno: '{negociacao.MesAnoRequerimento}', CPF/CNPJ: '{negociacao.CpfCnpjOptante}', Nome: '{negociacao.NomeOptante}', Conta: '{negociacao.NumeroContaNegociacao}')");
                            resultado.LinhasComErro++;
                            continue;
                        }

                        negociacoes.Add(negociacao);
                        resultado.LinhasImportadas++;
                    }
                    catch (Exception ex)
                    {
                        resultado.Erros.Add($"Linha {rowIndex + 1}: {ex.Message}");
                        resultado.LinhasComErro++;
                    }
                }

                // Salvar no banco usando BULK INSERT (muito mais rápido para grandes volumes)
                if (negociacoes.Count != 0)
                {
                    var agora = DateTime.UtcNow;

                    // Mapeamento de propriedades para colunas do banco (snake_case conforme convenção PostgreSQL)
                    // NOTA: Não incluir 'id' pois é IDENTITY (gerado automaticamente)
                    var columnMapping = new Dictionary<string, (string ColumnName, Func<NegociacaoFiscal, object?> ValueGetter)>
                    {
                        { "id_empresa", ("id_empresa", n => n.IdEmpresa) },
                        { "ativo", ("ativo", n => n.Ativo) },
                        { "data_cadastro", ("data_cadastro", n => agora) },
                        { "data_alteracao", ("data_alteracao", n => agora) },
                        { "criado_por_usuario_id", ("criado_por_usuario_id", n => n.CriadoPorUsuarioId) },
                        { "alterado_por_usuario_id", ("alterado_por_usuario_id", n => n.AlteradoPorUsuarioId) },
                        { "mes_ano_requerimento", ("mes_ano_requerimento", n => n.MesAnoRequerimento) },
                        { "ufoptante", ("ufoptante", n => n.UFOptante) },
                        { "cpf_cnpj_optante", ("cpf_cnpj_optante", n => n.CpfCnpjOptante) },
                        { "nome_optante", ("nome_optante", n => n.NomeOptante) },
                        { "numero_conta_negociacao", ("numero_conta_negociacao", n => n.NumeroContaNegociacao) },
                        { "tipo_negociacao", ("tipo_negociacao", n => n.TipoNegociacao) },
                        { "modalidade_negociacao", ("modalidade_negociacao", n => n.ModalidadeNegociacao) },
                        { "situacao_negociacao", ("situacao_negociacao", n => n.SituacaoNegociacao) },
                        { "qtde_parcelas_concedidas", ("qtde_parcelas_concedidas", n => n.QtdeParcelasConcedidas) },
                        { "qtde_parcelas_atraso", ("qtde_parcelas_atraso", n => n.QtdeParcelasAtraso) },
                        { "valor_consolidado", ("valor_consolidado", n => n.ValorConsolidado) },
                        { "valor_principal", ("valor_principal", n => n.ValorPrincipal) },
                        { "valor_multa", ("valor_multa", n => n.ValorMulta) },
                        { "valor_juros", ("valor_juros", n => n.ValorJuros) },
                        { "valor_encargo_legal", ("valor_encargo_legal", n => n.ValorEncargoLegal) }
                    };

                    // Usar bulk insert (PostgreSQL COPY FROM STDIN - extremamente rápido)
                    // Nome da tabela: negociacoes_fiscais (snake_case conforme migration InitialCreate)
                    await context.BulkInsertAsync(negociacoes, "negociacoes_fiscais", columnMapping);
                }

                resultado.Sucesso = resultado.LinhasImportadas > 0;
                resultado.Mensagem = resultado.Sucesso
                    ? $"Importação concluída! {resultado.LinhasImportadas} de {resultado.TotalLinhas} linhas importadas."
                    : "Nenhuma linha foi importada. Verifique os erros.";

                return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                {
                    Success = resultado.Sucesso,
                    Message = resultado.Mensagem,
                    Data = resultado,
                    Errors = resultado.Erros,
                    Warnings = resultado.Avisos,
                    Metadata = new Dictionary<string, object>
                    {
                        { "Total de Linhas", resultado.TotalLinhas },
                        { "Linhas Importadas", resultado.LinhasImportadas },
                        { "Linhas com Erro", resultado.LinhasComErro }
                    }
                };
            }
            catch (Exception ex)
            {
                resultado.Erros.Add(ex.ToString());
                return new ProcessingResult<ImportacaoNegociacaoFiscalResult>
                {
                    Success = false,
                    Message = $"Erro ao processar arquivo: {ex.Message}",
                    Data = resultado,
                    Errors = resultado.Erros,
                    Warnings = resultado.Avisos
                };
            }
        }

        private static object? GetCellValue(DataRow row, int col)
        {
            try
            {
                if (col >= row.Table.Columns.Count)
                {
                    return null;
                }

                var value = row[col];
                return value == DBNull.Value ? null : value;
            }
            catch
            {
                return null;
            }
        }

        private static string SafeString(object? value, int maxLength)
        {
            var stringValue = value?.ToString()?.Trim() ?? "";

            // Se string vazia, retornar vazio
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return "";
            }

            // Se exceder o tamanho máximo, truncar
            if (stringValue.Length > maxLength)
            {
                stringValue = stringValue.Substring(0, maxLength);
            }

            return stringValue;
        }

        private static string? SafeNullableString(object? value, int maxLength)
        {
            var stringValue = value?.ToString()?.Trim();

            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return null;
            }

            // Se exceder o tamanho máximo, truncar
            if (stringValue.Length > maxLength)
            {
                stringValue = stringValue.Substring(0, maxLength);
            }

            return stringValue;
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