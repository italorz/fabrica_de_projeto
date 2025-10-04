using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface ICondutorRepository : IBaseRepository<Condutor>
    {
        Task<Condutor?> GetByCPFAsync(string cpf);
        Task<Condutor?> GetByCNHAsync(string cnh);
    }
}
