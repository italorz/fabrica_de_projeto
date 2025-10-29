using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface IMultaService
    {
        Task<IEnumerable<Multa>> GetAllAsync();
        Task<Multa?> GetByIdAsync(int id);
        Task<IEnumerable<Multa>> GetByVeiculoIdAsync(int veiculoId);
        Task<IEnumerable<Multa>> GetByCondutorIdAsync(int condutorId);
        Task<IEnumerable<Multa>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Multa>> GetByTipoMultaIdAsync(int tipoMultaId);
        Task<IEnumerable<Multa>> GetByDateRangeAsync(DateTime dataInicio, DateTime dataFim);
        Task<Multa> CreateAsync(Multa multa);
        Task<Multa> UpdateAsync(Multa multa);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<Multa> ProcessarMultaFromAIAsync(string jsonData, int usuarioId);
    }
}
