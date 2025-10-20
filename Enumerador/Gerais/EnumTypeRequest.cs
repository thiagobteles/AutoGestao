namespace AutoGestao.Enumerador.Gerais
{
    /// <summary>
    /// Define o tipo de requisição HTTP para ações da grid
    /// </summary>
    public enum EnumTypeRequest
    {
        /// <summary>
        /// Requisição GET (padrão)
        /// </summary>
        Get = 0,

        /// <summary>
        /// Requisição POST (usado para Delete, Create, etc)
        /// </summary>
        Post = 1,

        /// <summary>
        /// Requisição PUT (usado para Update)
        /// </summary>
        Put = 2,

        /// <summary>
        /// Requisição DELETE (método HTTP DELETE)
        /// </summary>
        Delete = 3
    }
}