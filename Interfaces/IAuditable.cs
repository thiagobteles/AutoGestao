namespace AutoGestao.Interfaces
{
    /// <summary>
    /// Interface para entidades que devem ser auditadas
    /// </summary>
    public interface IAuditable
    {
        long Id { get; set; }

        DateTime DataCadastro { get; set; }

        DateTime DataAlteracao { get; set; }

        long? CriadoPorUsuarioId { get; set; }

        long? AlteradoPorUsuarioId { get; set; }
    }
}