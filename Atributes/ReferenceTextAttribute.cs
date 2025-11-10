using System;

namespace FGT.Atributes
{
    /// <summary>
    /// Define qual propriedade será usada como Text no ReferenceItem
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReferenceTextAttribute : Attribute
    {
        /// <summary>
        /// Indica se deve buscar relação (Include)
        /// </summary>
        public string? NavigationPath { get; set; }
    }

    /// <summary>
    /// Define qual propriedade será usada como Subtitle no ReferenceItem
    /// Pode ter múltiplos atributos com ordem de prioridade
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ReferenceSubtitleAttribute : Attribute
    {
        /// <summary>
        /// Ordem de exibição (0 = primeiro)
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Prefixo para exibir antes do valor
        /// Exemplo: "CPF: ", "Email: "
        /// </summary>
        public string? Prefix { get; set; }

        /// <summary>
        /// Indica se deve buscar relação (Include)
        /// Exemplo: [ReferenceSubtitle(NavigationPath = "VeiculoMarca.Descricao")]
        /// </summary>
        public string? NavigationPath { get; set; }

        /// <summary>
        /// Máscara de formatação
        /// Exemplo: "###.###.###-##" para CPF
        /// </summary>
        public string? Format { get; set; }
    }

    /// <summary>
    /// Define quais propriedades serão usadas na busca (Search)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ReferenceSearchableAttribute : Attribute
    {
        /// <summary>
        /// Indica se deve buscar relação (Include)
        /// Exemplo: [ReferenceSubtitle(NavigationPath = "VeiculoMarca.Descricao")]
        /// </summary>
        public string? NavigationPath { get; set; }
    }
}