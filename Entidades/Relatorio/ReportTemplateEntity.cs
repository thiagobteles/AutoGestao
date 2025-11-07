using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades.Relatorio
{
    [FormConfig(Title = "Template de Relatório", Subtitle = "Gerencie templates de relatórios salvos", Icon = "fas fa-file-alt")]
    public class ReportTemplateEntity : BaseEntidade
    {
        [GridField("Nome do Template", IsText = true, IsSearchable = true, IsLink = false, Order = 10)]
        [FormField(Order = 1, Name = "Nome", Section = "Dados Básicos", Icon = "fas fa-signature", Type = EnumFieldType.Text, Required = true, GridColumns = 2)]
        public string Nome { get; set; } = string.Empty;

        [FormField(Order = 1, Name = "Descrição", Section = "Dados Básicos", Icon = "fas fa-comment", Type = EnumFieldType.Text)]
        public string? Descricao { get; set; }

        public string TipoEntidade { get; set; } = string.Empty;

        [Column(TypeName = "text")]
        [FormField(Order = 10, Name = "Template JSON", Section = "Configuração", Icon = "fas fa-code", Type = EnumFieldType.TextArea, Required = true, GridColumns = 1)]
        public string TemplateJson { get; set; } = string.Empty;

        [GridField("Padrão", Order = 20, Width = "80px")]
        [FormField(Order = 20, Name = "Padrão", Section = "Status", Icon = "fas fa-star", Type = EnumFieldType.Checkbox, GridColumns = 2)]
        public bool Padrao { get; set; } = false;

        [FormField(Order = 21, Name = "Ativo", Section = "Status", Icon = "fas fa-toggle-on", Type = EnumFieldType.Checkbox)]
        public new bool Ativo { get; set; } = true;

        [NotMapped]
        public int TotalUsos { get; set; }
    }
}