using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email);
        Task<Usuario?> GetByCPFAsync(string cpf);
    }
}
