using AutoGestao.Attributes;

namespace AutoGestao.Entidades
{
    [Auditable(AuditCreate = true, AuditUpdate = true, AuditDelete = true)]
    public class BaseEntidadeEmpresa : BaseEntidade
    {
        public long IdEmpresa { get; set; }

        public bool Ativo { get; set; } = true;

        // Navigation properties para auditoria
        public virtual Empresa? Empresa { get; set; }
    }
}