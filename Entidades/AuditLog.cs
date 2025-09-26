using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Log de Auditoria", Subtitle = "Histórico completo de operações do sistema", Icon = "fas fa-history")]
    public class AuditLog : BaseEntidade
    {
        [FormField(DisplayName = "Usuário", Icon = "fas fa-user", Type = EnumFieldType.Text, ReadOnly = true, Order = 1, Section = "Identificação")]
        public int? UsuarioId { get; set; }

        [FormField(DisplayName = "Nome do Usuário", Icon = "fas fa-user", Type = EnumFieldType.Text, ReadOnly = true, Order = 2, Section = "Identificação")]
        public string? UsuarioNome { get; set; }

        [FormField(DisplayName = "Email do Usuário", Icon = "fas fa-envelope", Type = EnumFieldType.Email, ReadOnly = true, Order = 3, Section = "Identificação")]
        public string? UsuarioEmail { get; set; }

        [Required]
        [FormField(DisplayName = "Entidade", Icon = "fas fa-database", Type = EnumFieldType.Text, ReadOnly = true, Order = 10, Section = "Operação")]
        public string EntidadeNome { get; set; } = "";

        [FormField(DisplayName = "Nome Amigável", Icon = "fas fa-tag", Type = EnumFieldType.Text, ReadOnly = true, Order = 11, Section = "Operação")]
        public string? EntidadeDisplayName { get; set; }

        [Required]
        [FormField(DisplayName = "ID da Entidade", Icon = "fas fa-key", Type = EnumFieldType.Text, ReadOnly = true, Order = 12, Section = "Operação")]
        public string EntidadeId { get; set; } = "";

        [Required]
        [FormField(DisplayName = "Operação", Icon = "fas fa-cog", Type = EnumFieldType.Select, ReadOnly = true, Order = 13, Section = "Operação")]
        public EnumTipoOperacaoAuditoria TipoOperacao { get; set; }

        [FormField(DisplayName = "Tabela", Icon = "fas fa-table", Type = EnumFieldType.Text, ReadOnly = true, Order = 14, Section = "Operação")]
        public string? TabelaNome { get; set; }

        [FormField(DisplayName = "Valores Antigos", Icon = "fas fa-history", Type = EnumFieldType.TextArea, ReadOnly = true, Order = 20, Section = "Dados")]
        public string? ValoresAntigos { get; set; }

        [FormField(DisplayName = "Valores Novos", Icon = "fas fa-edit", Type = EnumFieldType.TextArea, ReadOnly = true, Order = 21, Section = "Dados")]
        public string? ValoresNovos { get; set; }

        [FormField(DisplayName = "Campos Alterados", Icon = "fas fa-list", Type = EnumFieldType.TextArea, ReadOnly = true, Order = 22, Section = "Dados")]
        public string? CamposAlterados { get; set; }

        [FormField(DisplayName = "IP do Cliente", Icon = "fas fa-globe", Type = EnumFieldType.Text, ReadOnly = true, Order = 30, Section = "Requisição")]
        public string? IpCliente { get; set; }

        [FormField(DisplayName = "User Agent", Icon = "fas fa-desktop", Type = EnumFieldType.Text, ReadOnly = true, Order = 31, Section = "Requisição")]
        public string? UserAgent { get; set; }

        [FormField(DisplayName = "URL da Requisição", Icon = "fas fa-link", Type = EnumFieldType.Text, ReadOnly = true, Order = 32, Section = "Requisição")]
        public string? UrlRequisicao { get; set; }

        [FormField(DisplayName = "Método HTTP", Icon = "fas fa-exchange-alt", Type = EnumFieldType.Text, ReadOnly = true, Order = 33, Section = "Requisição")]
        public string? MetodoHttp { get; set; }

        [FormField(DisplayName = "Sucesso", Icon = "fas fa-check-circle", Type = EnumFieldType.Checkbox, ReadOnly = true, Order = 40, Section = "Resultado")]
        public bool Sucesso { get; set; } = true;

        [FormField(DisplayName = "Mensagem de Erro", Icon = "fas fa-exclamation-triangle", Type = EnumFieldType.TextArea, ReadOnly = true, Order = 41, Section = "Resultado")]
        public string? MensagemErro { get; set; }

        [FormField(DisplayName = "Duração (ms)", Icon = "fas fa-clock", Type = EnumFieldType.Number, ReadOnly = true, Order = 42, Section = "Resultado")]
        public long? DuracaoMs { get; set; }

        [FormField(DisplayName = "Data/Hora", Icon = "fas fa-calendar", Type = EnumFieldType.DateTime, ReadOnly = true, Order = 50, Section = "Timestamp")]
        public DateTime DataHora { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Usuario? Usuario { get; set; }
    }
}