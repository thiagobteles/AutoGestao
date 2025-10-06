using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Tarefa", Subtitle = "Gerencie as tarefas e atividades", Icon = "fas fa-tasks", EnableAjaxSubmit = true)]
    public class Tarefa : BaseEntidade
    {
        [GridMain("Título")]
        [FormField(Order = 1, Name = "Título", Section = "Dados Básicos", Icon = "fas fa-heading", Type = EnumFieldType.Text, Required = true, GridColumns = 2)]
        public string Titulo { get; set; } = string.Empty;

        [FormField(Order = 1, Name = "Descrição", Section = "Dados Básicos", Icon = "fas fa-align-left", Type = EnumFieldType.TextArea, Placeholder = "Descreva a tarefa...")]
        public string? Descricao { get; set; }

        [GridField("Status", Order = 30, Width = "100px")]
        [FormField(Order = 10, Name = "Status", Section = "Informações", Icon = "fas fa-info-circle", Type = EnumFieldType.Select, Required = true, GridColumns = 2)]
        public EnumStatusTarefa Status { get; set; }

        [GridField("Prioridade", Order = 35, Width = "100px")]
        [FormField(Order = 10, Name = "Prioridade", Section = "Informações", Icon = "fas fa-exclamation", Type = EnumFieldType.Select, Required = true)]
        public EnumPrioridade Prioridade { get; set; }

        [GridField("Data Criação", Order = 20, Width = "110px")]
        [FormField(Order = 20, Name = "Data de Criação", Section = "Datas", Icon = "fas fa-calendar-plus", Type = EnumFieldType.Date, Required = true, ReadOnly = true, GridColumns = 3)]
        public DateTime DataCriacao { get; set; }

        [GridField("Vencimento", Order = 25, Width = "110px")]
        [FormField(Order = 20, Name = "Data de Vencimento", Section = "Datas", Icon = "fas fa-calendar", Type = EnumFieldType.Date)]
        public DateTime? DataVencimento { get; set; }

        [GridField("Conclusão", Order = 27, Width = "110px")]
        [FormField(Order = 20, Name = "Data de Conclusão", Section = "Datas", Icon = "fas fa-calendar-check", Type = EnumFieldType.Date)]
        public DateTime? DataConclusao { get; set; }

        [FormField(Order = 30, Name = "Responsável (Vendedor)", Section = "Responsabilidade", Icon = "fas fa-user-tie", Type = EnumFieldType.Reference, Reference = typeof(Vendedor), GridColumns = 2)]
        public long? IdResponsavel { get; set; }

        [FormField(Order = 30, Name = "Responsável (Usuário)", Section = "Responsabilidade", Icon = "fas fa-user", Type = EnumFieldType.Reference, Reference = typeof(Usuario), GridColumns = 2)]
        public long? IdResponsavelUsuario { get; set; }

        // Navigation properties
        public virtual Vendedor? Responsavel { get; set; }
        public virtual Usuario? ResponsavelUsuario { get; set; }
    }
}