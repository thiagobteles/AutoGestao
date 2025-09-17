using AutoGestao.Entidades.Veiculos;
using AutoGestao.Enumerador.Veiculo;

namespace AutoGestao.Models
{
    public class VeiculosIndexViewModel : BaseViewModel<Veiculo>
    {
        // Filtros Específicos para Veículos
        public EnumSituacaoVeiculo? Situacao { get; set; }
    }
}