using AutoGestao.Entidades;
using AutoGestao.Entidades.Veiculos;
using AutoGestao.Models;

namespace AutoGestao.Configuration
{
    /// <summary>
    /// Configurações específicas para campos de referência
    /// </summary>
    public static class ReferenceFieldConfiguration
    {
        /// <summary>
        /// Inicializa configurações padrão para Reference Fields
        /// </summary>
        public static void Initialize()
        {
            // Configurações para Cliente
            ReferenceFieldConfig.DefaultConfigs[typeof(Cliente)] = new ReferenceFieldConfig
            {
                SearchFields = ["Nome", "CPF", "Email"],
                SubtitleFields = ["CPF", "Email", "Telefone"],
                PageSize = 10,
                MinSearchLength = 2,
                AllowCreate = true
            };

            // Configurações para Fornecedor
            ReferenceFieldConfig.DefaultConfigs[typeof(Fornecedor)] = new ReferenceFieldConfig
            {
                SearchFields = ["Nome", "CNPJ", "Email"],
                SubtitleFields = ["CNPJ", "Email"],
                PageSize = 10,
                AllowCreate = true
            };

            // Configurações para Vendedor
            ReferenceFieldConfig.DefaultConfigs[typeof(Vendedor)] = new ReferenceFieldConfig
            {
                SearchFields = ["Nome", "CPF", "Email"],
                SubtitleFields = ["CPF", "Email"],
                PageSize = 15,
                AllowCreate = true,
                SearchFilters = new Dictionary<string, object> { ["Ativo"] = true }
            };

            // Configurações para VeiculoMarca
            ReferenceFieldConfig.DefaultConfigs[typeof(VeiculoMarca)] = new ReferenceFieldConfig
            {
                SearchFields = ["Descricao"],
                SubtitleFields = [],
                PageSize = 20,
                AllowCreate = true
            };

            // Configurações para VeiculoMarcaModelo
            ReferenceFieldConfig.DefaultConfigs[typeof(VeiculoMarcaModelo)] = new ReferenceFieldConfig
            {
                SearchFields = ["Descricao"],
                SubtitleFields = ["VeiculoMarca.Descricao"],
                PageSize = 15,
                AllowCreate = true
            };
        }
    }
}