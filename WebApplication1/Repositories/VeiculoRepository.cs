using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repositories
{
    public class VeiculoRepository : BaseRepository<Veiculo>, IVeiculoRepository
    {
        public VeiculoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Veiculo?> GetByPlacaAsync(string placa)
        {
            return await _dbSet.FirstOrDefaultAsync(v => v.Placa == placa);
        }

        public async Task<IEnumerable<Veiculo>> GetByProprietarioAsync(string proprietario)
        {
            return await _dbSet.Where(v => v.Proprietario != null && v.Proprietario.Contains(proprietario)).ToListAsync();
        }
    }
}
