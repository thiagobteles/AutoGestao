using AutoGestao.Atributes;
using AutoGestao.Models.Report;

namespace AutoGestao.Entidades.Relatorio
{
    // Configuração básica do relatório
    [ReportConfig("Relatório de Lançamento de Veículo", Subtitle = "Detalhes completos do lançamento", Icon = "fas fa-car", ShowLogo = true, ShowDate = true)]
    public class ReportVeiculo : BaseEntidade
    {
        // Campos básicos aparecerão na seção "Dados do Veículo"
        [ReportField("Modelo", Section = "AUTOMÓVEL", Order = 1)]
        public string Modelo { get; set; }

        [ReportField("Placa", Section = "AUTOMÓVEL", Order = 2)]
        public string Placa { get; set; }

        [ReportField("Tipo", Section = "AUTOMÓVEL", Order = 3)]
        public string Tipo { get; set; }

        [ReportField("Ano", Section = "AUTOMÓVEL", Order = 4)]
        public string Ano { get; set; }

        [ReportField("Chassi", Section = "AUTOMÓVEL", Order = 5)]
        public string Chassi { get; set; }

        [ReportField("Km", Section = "AUTOMÓVEL", Order = 6)]
        public string Km { get; set; }

        [ReportField("Cor", Section = "AUTOMÓVEL", Order = 7)]
        public string Cor { get; set; }

        [ReportField("Combustível", Section = "AUTOMÓVEL", Order = 8)]
        public string Combustivel { get; set; }

        [ReportField("Câmbio", Section = "AUTOMÓVEL", Order = 9)]
        public string Cambio { get; set; }

        // Tabela de receitas
        [ReportTable("Resumo das Receitas", ShowTotal = true, TotalField = "Valor", Order = 10)]
        public List<ReceitaItem> Receitas { get; set; } = [];

        // Tabela de lançamentos
        [ReportTable("Resumo dos Lançamentos", ShowTotal = true, TotalField = "Valor", Order = 11)]
        public List<LancamentoItem> Lancamentos { get; set; } = [];
    }

    public class ReceitaItem
    {
        public string DataServico { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public decimal Valor { get; set; }
    }

    public class LancamentoItem
    {
        public string DataCriacao { get; set; }
        public string Descricao { get; set; }
        public string Status { get; set; }
        public decimal Valor { get; set; }
    }
}