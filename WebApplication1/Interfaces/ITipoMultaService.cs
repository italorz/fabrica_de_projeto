using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface ITipoMultaService
    {
        Task<IEnumerable<TipoMulta>> GetAllAsync();
        Task<TipoMulta?> GetByIdAsync(int id);
        Task<TipoMulta?> GetByCodigoAsync(string codigo);
        Task<TipoMulta> CreateAsync(TipoMulta tipoMulta);
        Task<TipoMulta> UpdateAsync(TipoMulta tipoMulta);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> CodigoExistsAsync(string codigo);
        Task<IEnumerable<TipoMulta>> GetByGravidadeAsync(string gravidade);
    }
}
