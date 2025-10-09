using AutoGestao.Atributes;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades.Relatorio
{
    /// <summary>
    /// Entidade para salvar templates de relatório no banco
    /// </summary>
    [FormConfig(Title = "Template de Relatório", Subtitle = "Gerencie templates de relatórios salvos", Icon = "fas fa-file-alt")]
    public class ReportTemplateEntity : BaseEntidade
    {
        [GridMain("Nome do Template")]
        [FormField(Order = 1, Name = "Nome", Section = "Dados Básicos", Icon = "fas fa-signature", Type = EnumFieldType.Text, Required = true, GridColumns = 2)]
        public string Nome { get; set; } = string.Empty;

        [GridField("Tipo Entidade", Order = 10, Width = "150px")]
        [FormField(Order = 2, Name = "Tipo de Entidade", Section = "Dados Básicos", Icon = "fas fa-database", Type = EnumFieldType.Select, Required = true, GridColumns = 2)]
        public string TipoEntidade { get; set; } = string.Empty;

        [FormField(Order = 3, Name = "Descrição", Section = "Dados Básicos", Icon = "fas fa-comment", Type = EnumFieldType.TextArea, GridColumns = 1)]
        public string? Descricao { get; set; }

        [FormField(Order = 10, Name = "Template JSON", Section = "Configuração", Icon = "fas fa-code", Type = EnumFieldType.TextArea, Required = true, GridColumns = 1)]
        [Column(TypeName = "text")]
        public string TemplateJson { get; set; } = string.Empty;

        [GridField("Padrão", Order = 20, Width = "80px")]
        [FormField(Order = 20, Name = "Template Padrão", Section = "Configuração", Icon = "fas fa-star", Type = EnumFieldType.Checkbox, GridColumns = 1)]
        public bool IsPadrao { get; set; } = false;

        [GridField("Ativo", Order = 30, Width = "80px")]
        [FormField(Order = 30, Name = "Ativo", Section = "Status", Icon = "fas fa-toggle-on", Type = EnumFieldType.Checkbox, GridColumns = 1)]
        public new bool Ativo { get; set; } = true;

        [NotMapped]
        public int TotalUsos { get; set; } // Pode ser calculado via relatório
    }
}