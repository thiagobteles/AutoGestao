namespace AutoGestao.Interfaces
{
    /// <summary>
    /// Interface para entidades que devem ser auditadas
    /// </summary>
    public interface IAuditable
    {
        int Id { get; set; }
        DateTime DataCadastro { get; set; }
        DateTime DataAlteracao { get; set; }
        int? CriadoPorUsuarioId { get; set; }
        int? AlteradoPorUsuarioId { get; set; }
    }
}