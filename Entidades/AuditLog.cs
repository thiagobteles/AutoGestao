using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;

namespace AutoGestao.Entidades
{
    [FormConfig(Title = "Log de Auditoria", Subtitle = "Histórico completo de operações do sistema", Icon = "fas fa-history")]
    public class AuditLog : BaseEntidadeEmpresa
    {
        [FormField(Name = "Usuário", Icon = "fas fa-user", Type = EnumFieldType.Text, ReadOnly = true, Order = 1, Section = "Identificação")]
        public long? UsuarioId { get; set; }

        [FormField(Name = "Nome do Usuário", Icon = "fas fa-user", Type = EnumFieldType.Text, ReadOnly = true, Order = 2, Section = "Identificação")]
        public string? UsuarioNome { get; set; }

        [FormField(Name = "Email do Usuário", Icon = "fas fa-envelope", Type = EnumFieldType.Email, ReadOnly = true, Order = 3, Section = "Identificação")]
        public string? UsuarioEmail { get; set; }

        [FormField(Name = "Entidade", Icon = "fas fa-database", Type = EnumFieldType.Text, ReadOnly = true, Order = 10, Section = "Operação")]
        public string EntidadeNome { get; set; } = "";

        [FormField(Name = "Nome Amigável", Icon = "fas fa-tag", Type = EnumFieldType.Text, ReadOnly = true, Order = 11, Section = "Operação")]
        public string? EntidadeDisplayName { get; set; }

        [FormField(Name = "ID da Entidade", Icon = "fas fa-key", Type = EnumFieldType.Text, ReadOnly = true, Order = 12, Section = "Operação")]
        public string EntidadeId { get; set; } = "";

        [FormField(Name = "Operação", Icon = "fas fa-cog", Type = EnumFieldType.Select, ReadOnly = true, Order = 13, Section = "Operação")]
        public EnumTipoOperacaoAuditoria TipoOperacao { get; set; }

        [FormField(Name = "Tabela", Icon = "fas fa-table", Type = EnumFieldType.Text, ReadOnly = true, Order = 14, Section = "Operação")]
        public string? TabelaNome { get; set; }

        [FormField(Name = "Valores Antigos", Icon = "fas fa-history", Type = EnumFieldType.TextArea, ReadOnly = true, Order = 20, Section = "Dados")]
        public string? ValoresAntigos { get; set; }

        [FormField(Name = "Valores Novos", Icon = "fas fa-edit", Type = EnumFieldType.TextArea, ReadOnly = true, Order = 21, Section = "Dados")]
        public string? ValoresNovos { get; set; }

        [FormField(Name = "Campos Alterados", Icon = "fas fa-list", Type = EnumFieldType.TextArea, ReadOnly = true, Order = 22, Section = "Dados")]
        public string? CamposAlterados { get; set; }

        [FormField(Name = "IP do Cliente", Icon = "fas fa-globe", Type = EnumFieldType.Text, ReadOnly = true, Order = 30, Section = "Requisição")]
        public string? IpCliente { get; set; }

        [FormField(Name = "User Agent", Icon = "fas fa-desktop", Type = EnumFieldType.Text, ReadOnly = true, Order = 31, Section = "Requisição")]
        public string? UserAgent { get; set; }
        
        [FormField(Name = "URL da Requisição", Icon = "fas fa-link", Type = EnumFieldType.Text, ReadOnly = true, Order = 32, Section = "Requisição")]
        public string? UrlRequisicao { get; set; }

        [FormField(Name = "Método HTTP", Icon = "fas fa-exchange-alt", Type = EnumFieldType.Text, ReadOnly = true, Order = 33, Section = "Requisição")]
        public string? MetodoHttp { get; set; }

        [FormField(Name = "Sucesso", Icon = "fas fa-check-circle", Type = EnumFieldType.Checkbox, ReadOnly = true, Order = 40, Section = "Resultado")]
        public bool Sucesso { get; set; } = true;

        [FormField(Name = "Mensagem de Erro", Icon = "fas fa-exclamation-triangle", Type = EnumFieldType.TextArea, ReadOnly = true, Order = 41, Section = "Resultado")]
        public string? MensagemErro { get; set; }

        [FormField(Name = "Duração (ms)", Icon = "fas fa-clock", Type = EnumFieldType.Number, ReadOnly = true, Order = 42, Section = "Resultado")]
        public long? DuracaoMs { get; set; }

        [FormField(Name = "Data/Hora", Icon = "fas fa-calendar", Type = EnumFieldType.DateTime, ReadOnly = true, Order = 50, Section = "Timestamp")]
        public DateTime DataHora { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Usuario? Usuario { get; set; }
    }
}