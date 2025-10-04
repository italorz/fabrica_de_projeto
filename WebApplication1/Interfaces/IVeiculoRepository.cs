using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface IVeiculoRepository : IBaseRepository<Veiculo>
    {
        Task<Veiculo?> GetByPlacaAsync(string placa);
        Task<IEnumerable<Veiculo>> GetByProprietarioAsync(string proprietario);
    }
}
