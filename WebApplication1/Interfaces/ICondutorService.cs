using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface ICondutorService
    {
        Task<IEnumerable<Condutor>> GetAllAsync();
        Task<Condutor?> GetByIdAsync(int id);
        Task<Condutor?> GetByCPFAsync(string cpf);
        Task<Condutor?> GetByCNHAsync(string cnh);
        Task<Condutor> CreateAsync(Condutor condutor);
        Task<Condutor> UpdateAsync(Condutor condutor);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<bool> CPFExistsAsync(string cpf);
        Task<bool> CNHExistsAsync(string cnh);
    }
}
