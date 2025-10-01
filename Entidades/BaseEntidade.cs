using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Interfaces;

namespace AutoGestao.Entidades
{
    [Auditable(AuditCreate = true, AuditUpdate = true, AuditDelete = true)]
    public class BaseEntidade : IAuditable
    {
        public long Id { get; set; }

        [GridStatus]
        [FormField(Order = 10, Name = "Ativo", Section = "Status", Icon = "fas fa-toggle-on", Type = EnumFieldType.Checkbox)]
        public bool Ativo { get; set; } = true;

        [AuditIgnore]
        public DateTime DataCadastro { get; set; }

        [AuditIgnore]
        public DateTime DataAlteracao { get; set; }

        [AuditIgnore]
        public long? CriadoPorUsuarioId { get; set; }

        [AuditIgnore]
        public long? AlteradoPorUsuarioId { get; set; }

        // Navigation properties para auditoria
        public virtual Usuario? CriadoPorUsuario { get; set; }
        public virtual Usuario? AlteradoPorUsuario { get; set; }
    }
}