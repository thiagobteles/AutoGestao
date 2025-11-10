namespace FGT.Interfaces
{
    /// <summary>
    /// Interface base para todas as entidades do sistema
    /// </summary>
    public interface IEntity
    {
        long Id { get; set; }

        long IdEmpresa { get; set; }
    }
}
