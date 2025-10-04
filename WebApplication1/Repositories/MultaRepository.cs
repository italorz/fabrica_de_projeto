using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repositories
{
    public class MultaRepository : BaseRepository<Multa>, IMultaRepository
    {
        public MultaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Multa>> GetByVeiculoIdAsync(int veiculoId)
        {
            return await _dbSet.Where(m => m.VeiculoId == veiculoId)
                              .Include(m => m.TipoMulta)
                              .Include(m => m.Condutor)
                              .Include(m => m.Usuario)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Multa>> GetByCondutorIdAsync(int condutorId)
        {
            return await _dbSet.Where(m => m.CondutorId == condutorId)
                              .Include(m => m.TipoMulta)
                              .Include(m => m.Veiculo)
                              .Include(m => m.Usuario)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Multa>> GetByUsuarioIdAsync(int usuarioId)
        {
            return await _dbSet.Where(m => m.UsuarioId == usuarioId)
                              .Include(m => m.TipoMulta)
                              .Include(m => m.Veiculo)
                              .Include(m => m.Condutor)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Multa>> GetByTipoMultaIdAsync(int tipoMultaId)
        {
            return await _dbSet.Where(m => m.TipoMultaId == tipoMultaId)
                              .Include(m => m.Veiculo)
                              .Include(m => m.Condutor)
                              .Include(m => m.Usuario)
                              .ToListAsync();
        }

        public async Task<IEnumerable<Multa>> GetByDataAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _dbSet.Where(m => m.DataHora >= dataInicio && m.DataHora <= dataFim)
                              .Include(m => m.TipoMulta)
                              .Include(m => m.Veiculo)
                              .Include(m => m.Condutor)
                              .Include(m => m.Usuario)
                              .ToListAsync();
        }
    }
}
