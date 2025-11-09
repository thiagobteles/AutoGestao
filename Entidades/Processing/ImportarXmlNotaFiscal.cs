using AutoGestao.Atributes;
using AutoGestao.Data;
using AutoGestao.Entidades.Fiscal;
using AutoGestao.Enumerador.Fiscal;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Services.Interface;
using System.Globalization;
using System.Xml.Linq;

using Microsoft.EntityFrameworkCore;

namespace AutoGestao.Entidades.Processing
{
    /// <summary>
    /// Processamento para importar XML de Nota Fiscal Eletrônica (NFe)
    /// Exemplo de tela de processamento genérica
    /// </summary>
    [ProcessingForm(
        Title = "Importar XML de Nota Fiscal",
        Icon = "fas fa-file-import",
        Description = "Importe notas fiscais eletrônicas a partir de arquivos XML",
        SubmitButtonText = "Importar Nota Fiscal",
        SubmitButtonIcon = "fas fa-upload",
        ShowConfirmation = true,
        ConfirmationMessage = "Deseja importar esta nota fiscal? Os dados serão extraídos do arquivo XML e uma nova nota será criada no sistema.",
        AllowMultipleExecutions = true,
        RedirectOnSuccessUrl = "/NotaFiscal/Index"
    )]
    public class ImportarXmlNotaFiscal : IProcessingEntity<NotaFiscal>
    {
        [FormField(Name = "Arquivo XML da nota fiscal eletrônica", Order = 10, Section = "Arquivo", Icon = "fas fa-file-code", Type = EnumFieldType.File, Required = true, AllowedExtensions = "xml", MaxSizeMB = 5, GridColumns = 1)]
        public string? ArquivoXml { get; set; }

        [FormField(Name = "Empresa", Order = 20, Section = "Configurações", Icon = "fas fa-building", Type = EnumFieldType.Reference, Reference = typeof(EmpresaCliente), Required = true, HelpText = "Selecione a empresa que está recebendo/emitindo esta nota", GridColumns = 3)]
        public long EmpresaClienteId { get; set; }

        [FormField(Name = "Sobrescrever se já existir", Order = 30, Section = "Configurações", Icon = "fas fa-sync", Type = EnumFieldType.Checkbox, HelpText = "sobrescrever os dados de uma nota com a mesma chave de acesso")]
        public bool SobrescreverSeExistir { get; set; } = false;

        [FormField(Name = "Validar antes de importar", Order = 40, Section = "Configurações", Icon = "fas fa-check-circle", Type = EnumFieldType.Checkbox, HelpText = "Validar a estrutura do XML antes de importar (recomendado)")]
        public bool ValidarAntes { get; set; } = true;

        public async Task<ProcessingResult<NotaFiscal>> ProcessAsync(ApplicationDbContext context, IFileStorageService fileStorageService, long userId, long empresaId)
        {
            try
            {
                // 1. Validar que o arquivo foi fornecido
                if (string.IsNullOrEmpty(ArquivoXml))
                {
                    return ProcessingResult<NotaFiscal>.Fail("Arquivo XML não foi fornecido");
                }

                // 2. Baixar o arquivo do MinIO
                var stream = await fileStorageService.DownloadFileAsync(ArquivoXml, nameof(ImportarXmlNotaFiscal), empresaId);
                if (stream == null)
                {
                    return ProcessingResult<NotaFiscal>.Fail("Não foi possível baixar o arquivo XML");
                }

                // 3. Parsear o XML
                XDocument xmlDoc;
                try
                {
                    xmlDoc = XDocument.Load(stream);
                }
                catch (Exception ex)
                {
                    return ProcessingResult<NotaFiscal>.Fail("Erro ao ler o arquivo XML", [ex.Message]);
                }
                finally
                {
                    await stream.DisposeAsync();
                }

                // 4. Validar estrutura básica
                if (ValidarAntes)
                {
                    var validacao = ValidarXmlNFe(xmlDoc);
                    if (!string.IsNullOrEmpty(validacao))
                    {
                        return ProcessingResult<NotaFiscal>.Fail("XML inválido", [validacao]);
                    }
                }

                // 5. Extrair dados do XML
                var nfe = ExtrairDadosNFe(xmlDoc);

                // 6. Verificar se já existe nota com essa chave
                var notaExistente = await context.NotasFiscais.FirstOrDefaultAsync(n => n.ChaveAcesso == nfe.ChaveAcesso);
                if (notaExistente != null && !SobrescreverSeExistir)
                {
                    return ProcessingResult<NotaFiscal>.Fail(
                        $"Já existe uma nota fiscal com a chave de acesso {nfe.ChaveAcesso}",
                        ["Habilite a opção 'Sobrescrever se já existir' para atualizar a nota existente"]
                    );
                }

                // 7. Criar ou atualizar nota fiscal
                NotaFiscal notaFiscal;
                bool isNova = false;

                if (notaExistente != null)
                {
                    // Atualizar nota existente
                    notaFiscal = notaExistente;
                    AtualizarNotaFiscal(notaFiscal, nfe, EmpresaClienteId, ArquivoXml);
                }
                else
                {
                    // Criar nova nota
                    notaFiscal = CriarNotaFiscal(nfe, EmpresaClienteId, empresaId, ArquivoXml);
                    isNova = true;
                }

                // 8. Salvar no banco
                if (isNova)
                {
                    context.NotasFiscais.Add(notaFiscal);
                }

                await context.SaveChangesAsync();

                // 9. Retornar resultado com metadados
                var result = ProcessingResult<NotaFiscal>.Ok(
                    isNova
                        ? $"Nota fiscal #{notaFiscal.Numero} importada com sucesso!"
                        : $"Nota fiscal #{notaFiscal.Numero} atualizada com sucesso!",
                    notaFiscal
                );

                result.Metadata["NotaFiscalId"] = notaFiscal.Id;
                result.Metadata["Numero"] = notaFiscal.Numero.ToString();
                result.Metadata["Serie"] = notaFiscal.Serie.ToString();
                result.Metadata["ValorTotal"] = notaFiscal.ValorTotal.ToString("C", new CultureInfo("pt-BR"));
                result.Metadata["ChaveAcesso"] = notaFiscal.ChaveAcesso ?? "N/A";
                result.Metadata["Operacao"] = isNova ? "Importação" : "Atualização";

                return result;
            }
            catch (Exception ex)
            {
                return ProcessingResult<NotaFiscal>.Fail("Erro ao processar importação", [ex.Message, ex.StackTrace ?? ""]);
            }
        }

        #region Métodos Auxiliares

        private static string? ValidarXmlNFe(XDocument xml)
        {
            // Namespace padrão da NFe
            XNamespace nfe = "http://www.portalfiscal.inf.br/nfe";

            // Verificar se tem o nó raiz nfeProc ou NFe
            var nfeProc = xml.Root?.Element(nfe + "NFe");
            nfeProc ??= xml.Root;

            if (nfeProc == null)
            {
                return "Estrutura de XML inválida: não encontrado nó raiz NFe";
            }

            // Verificar se tem infNFe
            var infNfe = nfeProc.Descendants(nfe + "infNFe").FirstOrDefault();
            return infNfe == null 
                ? "Estrutura de XML inválida: não encontrado nó infNFe"
                : null;
        }

        private static DadosNFe ExtrairDadosNFe(XDocument xml)
        {
            XNamespace nfe = "http://www.portalfiscal.inf.br/nfe";

            var infNfe = xml.Descendants(nfe + "infNFe").FirstOrDefault();
            if (infNfe == null)
            {
                throw new InvalidOperationException("Nó infNFe não encontrado");
            }

            // Extrair chave de acesso do atributo Id (remove "NFe")
            var chaveAcesso = infNfe.Attribute("Id")?.Value.Replace("NFe", "") ?? "";

            // Identificação da NFe
            var ide = infNfe.Element(nfe + "ide");
            var numero = int.Parse(ide?.Element(nfe + "nNF")?.Value ?? "0");
            var serie = int.Parse(ide?.Element(nfe + "serie")?.Value ?? "1");
            var modelo = int.Parse(ide?.Element(nfe + "mod")?.Value ?? "55");
            var tipoNf = int.Parse(ide?.Element(nfe + "tpNF")?.Value ?? "1");
            var dataEmissao = DateTime.Parse(ide?.Element(nfe + "dhEmi")?.Value ?? DateTime.Now.ToString());

            // Totais
            var total = infNfe.Element(nfe + "total")?.Element(nfe + "ICMSTot");
            var valorTotal = decimal.Parse(total?.Element(nfe + "vNF")?.Value ?? "0", CultureInfo.InvariantCulture);
            var valorProdutos = decimal.Parse(total?.Element(nfe + "vProd")?.Value ?? "0", CultureInfo.InvariantCulture);
            var valorICMS = decimal.Parse(total?.Element(nfe + "vICMS")?.Value ?? "0", CultureInfo.InvariantCulture);
            var valorIPI = decimal.Parse(total?.Element(nfe + "vIPI")?.Value ?? "0", CultureInfo.InvariantCulture);
            var valorPIS = decimal.Parse(total?.Element(nfe + "vPIS")?.Value ?? "0", CultureInfo.InvariantCulture);
            var valorCOFINS = decimal.Parse(total?.Element(nfe + "vCOFINS")?.Value ?? "0", CultureInfo.InvariantCulture);

            return new DadosNFe
            {
                ChaveAcesso = chaveAcesso,
                Numero = numero,
                Serie = serie,
                Modelo = modelo == 55 ? EnumModeloNotaFiscal.NFe : EnumModeloNotaFiscal.NFCe,
                Tipo = tipoNf == 0 ? EnumTipoNotaFiscal.Entrada : EnumTipoNotaFiscal.Saida,
                DataEmissao = dataEmissao,
                ValorTotal = valorTotal,
                ValorProdutos = valorProdutos,
                ValorICMS = valorICMS,
                ValorIPI = valorIPI,
                ValorPIS = valorPIS,
                ValorCOFINS = valorCOFINS
            };
        }

        private static NotaFiscal CriarNotaFiscal(DadosNFe nfe, long empresaClienteId, long empresaId, string arquivoXml)
        {
            return new NotaFiscal
            {
                ChaveAcesso = nfe.ChaveAcesso,
                Numero = nfe.Numero,
                Serie = nfe.Serie,
                Modelo = nfe.Modelo,
                Tipo = nfe.Tipo,
                DataEmissao = nfe.DataEmissao,
                DataSaidaEntrada = nfe.DataEmissao,
                ValorTotal = nfe.ValorTotal,
                ValorProdutos = nfe.ValorProdutos,
                ValorServicos = 0,
                ValorICMS = nfe.ValorICMS,
                ValorIPI = nfe.ValorIPI,
                ValorPIS = nfe.ValorPIS,
                ValorCOFINS = nfe.ValorCOFINS,
                EmpresaClienteId = empresaClienteId,
                Status = EnumStatusNotaFiscal.Emitida,
                ArquivoXML = arquivoXml,
                IdEmpresa = empresaId,
                Ativo = true
            };
        }

        private static void AtualizarNotaFiscal(NotaFiscal notaFiscal, DadosNFe nfe, long empresaClienteId, string arquivoXml)
        {
            notaFiscal.ChaveAcesso = nfe.ChaveAcesso;
            notaFiscal.Numero = nfe.Numero;
            notaFiscal.Serie = nfe.Serie;
            notaFiscal.Modelo = nfe.Modelo;
            notaFiscal.Tipo = nfe.Tipo;
            notaFiscal.DataEmissao = nfe.DataEmissao;
            notaFiscal.DataSaidaEntrada = nfe.DataEmissao;
            notaFiscal.ValorTotal = nfe.ValorTotal;
            notaFiscal.ValorProdutos = nfe.ValorProdutos;
            notaFiscal.ValorICMS = nfe.ValorICMS;
            notaFiscal.ValorIPI = nfe.ValorIPI;
            notaFiscal.ValorPIS = nfe.ValorPIS;
            notaFiscal.ValorCOFINS = nfe.ValorCOFINS;
            notaFiscal.EmpresaClienteId = empresaClienteId;
            notaFiscal.ArquivoXML = arquivoXml;
        }

        #endregion

        #region Classes Auxiliares

        private class DadosNFe
        {
            public string ChaveAcesso { get; set; } = string.Empty;
            public int Numero { get; set; }
            public int Serie { get; set; }
            public EnumModeloNotaFiscal Modelo { get; set; }
            public EnumTipoNotaFiscal Tipo { get; set; }
            public DateTime DataEmissao { get; set; }
            public decimal ValorTotal { get; set; }
            public decimal ValorProdutos { get; set; }
            public decimal ValorICMS { get; set; }
            public decimal ValorIPI { get; set; }
            public decimal ValorPIS { get; set; }
            public decimal ValorCOFINS { get; set; }
        }

        #endregion
    }
}
