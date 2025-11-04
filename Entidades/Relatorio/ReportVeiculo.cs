namespace AutoGestao.Entidades.Relatorio
{
    public class ReportVeiculo : BaseEntidade
    {
        public string Modelo { get; set; }

        public string Placa { get; set; }

        public string Tipo { get; set; }

        public string Ano { get; set; }

        public string Chassi { get; set; }

        public string Km { get; set; }

        public string Cor { get; set; }

        public string Combustivel { get; set; }

        public string Cambio { get; set; }

        // Tabela de receitas
        public List<ReceitaItem> Receitas { get; set; } = [];

        // Tabela de lançamentos
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