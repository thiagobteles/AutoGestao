using AutoGestao.Atributes;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoGestao.Entidades.Fiscal
{
    [FormConfig(Title = "Certificado Digital", Subtitle = "Gerencie os certificados digitais A1 das empresas", Icon = "fas fa-certificate")]
    public class CertificadoDigital : BaseEntidade
    {
        [FormField(Name = "Empresa", Order = 10, Section = "Dados do Certificado", Icon = "fas fa-building", Type = EnumFieldType.Reference, Required = true, Reference = typeof(EmpresaCliente))]
        [Required]
        public long EmpresaClienteId { get; set; }

        [GridComposite("Empresa", Order = 10, NavigationPaths = new[] { "EmpresaCliente.RazaoSocial", "EmpresaCliente.CNPJ" },
            Template = @"<div class=""vehicle-info""><div class=""fw-semibold"">{0}</div><div class=""text-muted small"">{1}</div></div>")]
        public string EmpresaClienteNome => $"{EmpresaCliente?.RazaoSocial ?? "N/A"} - {EmpresaCliente?.CNPJ ?? "N/A"}";

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

        // Navigation
        [ForeignKey("EmpresaClienteId")]
        public virtual EmpresaCliente? EmpresaCliente { get; set; }
    }
}
