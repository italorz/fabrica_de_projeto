using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repositories
{
    public class TipoMultaRepository : BaseRepository<TipoMulta>, ITipoMultaRepository
    {
        public TipoMultaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<TipoMulta?> GetByCodigoAsync(string codigo)
        {
            return await _dbSet.FirstOrDefaultAsync(t => t.CodigoMulta == codigo);
        }

        public async Task<IEnumerable<TipoMulta>> GetByGravidadeAsync(string gravidade)
        {
            return await _dbSet.Where(t => t.Gravidade == gravidade).ToListAsync();
        }
    }
}
