using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades.Fiscal
{
    [FormConfig(Title = "Certificado Digital", Subtitle = "Gerencie os certificados digitais A1 das empresas", Icon = "fas fa-certificate")]
    public class CertificadoDigital : BaseEntidade
    {
        [GridField("Empresa", Order = 10)]
        [FormField(Name = "Empresa", Order = 10, Section = "Dados do Certificado", Icon = "fas fa-building", Type = EnumFieldType.Reference, Required = true, ReferenceEntity = "EmpresaCliente")]
        [Required]
        public int EmpresaClienteId { get; set; }

        [GridField("Titular", Order = 15)]
        [FormField(Name = "Titular", Order = 15, Section = "Dados do Certificado", Icon = "fas fa-user", Type = EnumFieldType.Text, Required = true)]
        [Required]
        [MaxLength(200)]
        public string Titular { get; set; } = string.Empty;

        [GridField("Validade", Order = 20, Width = "120px", Format = "dd/MM/yyyy")]
        [FormField(Name = "Data de Validade", Order = 20, Section = "Dados do Certificado", Icon = "fas fa-calendar-times", Type = EnumFieldType.Date, Required = true)]
        [Required]
        public DateTime DataValidade { get; set; }

        [FormField(Name = "Senha do Certificado", Order = 25, Section = "Segurança", Icon = "fas fa-lock", Type = EnumFieldType.Password)]
        [MaxLength(50)]
        public string? Senha { get; set; }

        [FormField(Name = "Arquivo do Certificado (.pfx)", Order = 30, Section = "Arquivo", Icon = "fas fa-file-certificate", Type = EnumFieldType.File, AllowedExtensions = "pfx,p12", MaxSizeMB = 5)]
        [MaxLength(500)]
        public string? ArquivoCertificado { get; set; }

        [GridField("Ativo", Order = 40, Width = "80px", EnumRender = EnumRenderType.Badge)]
        [FormField(Name = "Ativo", Order = 40, Section = "Status", Icon = "fas fa-toggle-on", Type = EnumFieldType.Boolean)]
        public bool Ativo { get; set; } = true;

        // Navigation
        [ForeignKey("EmpresaClienteId")]
        public virtual EmpresaCliente? EmpresaCliente { get; set; }
    }
}
