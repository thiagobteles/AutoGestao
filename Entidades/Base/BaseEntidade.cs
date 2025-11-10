using FGT.Atributes;
using FGT.Interfaces;

namespace FGT.Entidades.Base
{
    [Auditable(AuditCreate = true, AuditUpdate = true, AuditDelete = true)]
    public class BaseEntidade : IEntity, IAuditable
    {
        public long Id { get; set; }

        public long IdEmpresa { get; set; }

        [GridStatus]
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
        public virtual Empresa? Empresa { get; set; }
    }
}