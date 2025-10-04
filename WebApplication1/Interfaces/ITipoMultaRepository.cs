using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface ITipoMultaRepository : IBaseRepository<TipoMulta>
    {
        Task<TipoMulta?> GetByCodigoAsync(string codigo);
        Task<IEnumerable<TipoMulta>> GetByGravidadeAsync(string gravidade);
    }
}
