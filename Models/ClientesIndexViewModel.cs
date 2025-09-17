using AutoGestao.Entidades;
using AutoGestao.Enumerador;

namespace AutoGestao.Models
{
    public class ClientesIndexViewModel : BaseViewModel<Cliente>
    {
        // Filtros Específicos
        public EnumTipoPessoa? TipoCliente { get; set; }

        public bool? Ativo { get; set; }
    }
}