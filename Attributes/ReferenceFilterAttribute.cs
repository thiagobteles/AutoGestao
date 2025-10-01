using AutoGestao.Enumerador.Gerais;
using System;

namespace AutoGestao.Attributes
{
    /// <summary>
    /// Atributo para definir filtros dinâmicos em campos de referência.
    /// Permite filtrar a seleção de um campo Reference baseado em outro campo da entidade.
    /// </summary>
    /// <example>
    /// Filtrar por outro campo da entidade:
    /// [ReferenceFilter("IdVeiculoMarca", "IdVeiculoMarca")]
    /// public long IdVeiculoMarcaModelo { get; set; }
    /// 
    /// Filtrar por valor fixo:
    /// [ReferenceFilter("Status", "1")]
    /// public long IdCategoria { get; set; }
    /// 
    /// Múltiplos filtros:
    /// [ReferenceFilter("IdVeiculoMarca", "IdVeiculoMarca")]
    /// [ReferenceFilter("Ativo", "true")]
    /// public long IdVeiculoMarcaModelo { get; set; }
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ReferenceFilterAttribute : Attribute
    {
        /// <summary>
        /// Nome do campo da entidade de referência que será filtrado
        /// </summary>
        public string FilterField { get; }

        /// <summary>
        /// Valor do filtro. Pode ser:
        /// - Nome de uma propriedade da entidade atual (ex: "IdVeiculoMarca")
        /// - Valor fixo (ex: "1", "true", "Ativo")
        /// </summary>
        public string FilterValue { get; }

        /// <summary>
        /// Tipo de comparação. Padrão: Equals
        /// </summary>
        public EnumFilterOperator Operator { get; set; } = EnumFilterOperator.Equals;

        /// <summary>
        /// Se true, o valor é uma propriedade da entidade.
        /// Se false, é um valor literal.
        /// Padrão: true (tenta detectar automaticamente)
        /// </summary>
        public bool IsPropertyReference { get; set; } = true;

        public ReferenceFilterAttribute(string filterField, string filterValue)
        {
            if (string.IsNullOrWhiteSpace(filterField))
            {
                throw new ArgumentException("FilterField não pode ser vazio", nameof(filterField));
            }

            if (string.IsNullOrWhiteSpace(filterValue))
            {
                throw new ArgumentException("FilterValue não pode ser vazio", nameof(filterValue));
            }

            FilterField = filterField;
            FilterValue = filterValue;
        }
    }
}