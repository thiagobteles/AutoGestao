using AutoGestao.Atributes;
using AutoGestao.Attributes;
using AutoGestao.Enumerador.Gerais;
using AutoGestao.Interfaces;

namespace AutoGestao.Entidades
{
    [Auditable(AuditCreate = true, AuditUpdate = true, AuditDelete = true)]
    public class BaseEntidade : IAuditable
    {
        public int Id { get; set; }

        [AuditIgnore]
        public DateTime DataCadastro { get; set; }

        [AuditIgnore]
        public DateTime DataAlteracao { get; set; }

        [AuditIgnore]
        public int? CriadoPorUsuarioId { get; set; }

        [AuditIgnore]
        public int? AlteradoPorUsuarioId { get; set; }

        // Navigation properties para auditoria
        public virtual Usuario? CriadoPorUsuario { get; set; }
        public virtual Usuario? AlteradoPorUsuario { get; set; }
    }
}