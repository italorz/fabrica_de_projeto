using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface IMultaRepository : IBaseRepository<Multa>
    {
        Task<IEnumerable<Multa>> GetByVeiculoIdAsync(int veiculoId);
        Task<IEnumerable<Multa>> GetByCondutorIdAsync(int condutorId);
        Task<IEnumerable<Multa>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Multa>> GetByTipoMultaIdAsync(int tipoMultaId);
        Task<IEnumerable<Multa>> GetByDataAsync(DateTime dataInicio, DateTime dataFim);
    }
}
