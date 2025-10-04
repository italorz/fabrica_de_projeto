using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Repositories
{
    public class CondutorRepository : BaseRepository<Condutor>, ICondutorRepository
    {
        public CondutorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Condutor?> GetByCPFAsync(string cpf)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.CPF == cpf);
        }

        public async Task<Condutor?> GetByCNHAsync(string cnh)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.CNH == cnh);
        }
    }
}
