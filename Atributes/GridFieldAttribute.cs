using AutoGestao.Entidades;
using AutoGestao.Enumerador;
using AutoGestao.Enumerador.Gerais;
using System;

namespace AutoGestao.Atributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GridFieldAttribute(string? displayName = null) : Attribute
    {
        /// <summary>
        /// Nome de exibição na grid e nos labels
        /// </summary>
        public string? DisplayName { get; set; } = displayName;

        /// <summary>
        /// Ordem de exibição (menor = primeiro)
        /// Se não informado, usa ordem alfabética inteligente
        /// </summary>
        public int Order { get; set; } = -1; // -1 = auto

        /// <summary>
        /// Largura da coluna (ex: "120px", "15%")
        /// Se não informado, calcula automaticamente
        /// </summary>
        public string? Width { get; set; }

        /// <summary>
        /// Se a coluna é ordenável
        /// </summary>
        public bool Sortable { get; set; } = true;

        /// <summary>
        /// Se deve criar link para Details na grid
        /// </summary>
        public bool IsLink { get; set; } = false;

        /// <summary>
        /// Se este campo é o TEXTO PRINCIPAL do ReferenceItem
        /// Apenas UMA propriedade deve ter IsText = true
        /// </summary>
        public bool IsText { get; set; } = false;

        /// <summary>
        /// Se este campo é BUSCÁVEL (ReferenceSearchable)
        /// </summary>
        public bool IsSearchable { get; set; } = false;

        /// <summary>
        /// Se este campo aparece no SUBTITLE do ReferenceItem
        /// </summary>
        public bool IsSubtitle { get; set; } = false;

        /// <summary>
        /// Prefixo para o subtitle (ex: "CPF: ", "Email: ")
        /// </summary>
        public string? SubtitlePrefix { get; set; }

        /// <summary>
        /// Ordem no subtitle (quando IsSubtitle = true)
        /// </summary>
        public int SubtitleOrder { get; set; } = 0;

        /// <summary>
        /// Máscara de formatação
        /// Exemplos: "###.###.###-##" (CPF), "##.###.###/####-##" (CNPJ), C "Financeiro"
        /// </summary>
        public string? Format { get; set; }

        /// <summary>
        /// Navegação para relacionamento (ex: "VeiculoMarca.Descricao")
        /// </summary>
        public string? NavigationPath { get; set; }

        /// <summary>
        /// Se false, campo NÃO aparece na grid (mas pode aparecer no ReferenceItem)
        /// </summary>
        public bool ShowInGrid { get; set; } = true;

        public EnumDocumentType DocumentType { get; set; }

        public EnumRenderType EnumRender { get; set; } = EnumRenderType.Description;
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GridIdAttribute : GridFieldAttribute
    {
        public GridIdAttribute(string? displayName = "Cód") : base(displayName)
        {
            Order = 0;
            Width = "65px";
            Sortable = true;
            ShowInGrid = true;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GridStatusAttribute : GridFieldAttribute
    {
        public GridStatusAttribute(string? displayName = "Ativo") : base(displayName)
        {
            Order = 999;
            Width = "65px";
            Sortable = true;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GridMainAttribute : GridFieldAttribute
    {
        public GridMainAttribute(string? displayName = null) : base(displayName)
        {
            IsText = true;
            IsSearchable = true;
            IsLink = true;
            Order = 10;
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GridDocumentAttribute : GridFieldAttribute
    {
        public GridDocumentAttribute(string? displayName = null, EnumDocumentType type = EnumDocumentType.CPF, int order = 30) : base(displayName)
        {
            IsSubtitle = true;
            IsSearchable = true;
            SubtitleOrder = 0;
            Order = order;

            if (type == EnumDocumentType.CPF)
            {
                SubtitlePrefix = "CPF: ";
                Format = "###.###.###-##";
                DisplayName = displayName ?? "CPF";
            }
            else if (type == EnumDocumentType.CNPJ)
            {
                SubtitlePrefix = "CNPJ: ";
                Format = "##.###.###/####-##";
                DisplayName = displayName ?? "CNPJ";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class GridContactAttribute : GridFieldAttribute
    {
        public GridContactAttribute(string? displayName = null) : base(displayName)
        {
            IsSubtitle = true;
            IsSearchable = true;
            SubtitleOrder = 1;
            Order = 50;
        }
    }
}