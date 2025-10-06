using System;

namespace AutoGestao.Atributes
{
    /// <summary>
    /// Atributo para marcar entidades que devem ser auditadas
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AuditableAttribute : Attribute
    {
        public bool AuditCreate { get; set; } = true;
        public bool AuditUpdate { get; set; } = true;
        public bool AuditDelete { get; set; } = true;
        public string EntityDisplayName { get; set; } = "";
    }

    /// <summary>
    /// Atributo para ignorar campos específicos na auditoria
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AuditIgnoreAttribute : Attribute
    {
    }

    /// <summary>
    /// Atributo para marcar campos sensíveis (senhas, etc)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class AuditSensitiveAttribute : Attribute
    {
        public string MaskPattern { get; set; } = "***";
    }
}