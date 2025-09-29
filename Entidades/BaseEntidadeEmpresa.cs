using AutoGestao.Attributes;

namespace AutoGestao.Entidades
{
    [Auditable(AuditCreate = true, AuditUpdate = true, AuditDelete = true)]
    public class BaseEntidadeEmpresa : BaseEntidade
    {
        public int? EmpresaId { get; set; }

        // Navigation properties para auditoria
        public virtual Empresa? Empresa { get; set; }
    }
}