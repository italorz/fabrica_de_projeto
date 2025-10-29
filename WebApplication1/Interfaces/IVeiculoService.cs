using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface IVeiculoService
    {
        Task<IEnumerable<Veiculo>> GetAllAsync();
        Task<Veiculo?> GetByIdAsync(int id);
        Task<Veiculo?> GetByPlacaAsync(string placa);
        Task<Veiculo> CreateAsync(Veiculo veiculo);
        Task<Veiculo> UpdateAsync(Veiculo veiculo);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> PlacaExistsAsync(string placa);
        Task<IEnumerable<Veiculo>> SearchByPlacaAsync(string placa);
    }
}
