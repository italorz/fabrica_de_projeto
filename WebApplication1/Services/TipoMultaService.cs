using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Services
{
    public class TipoMultaService : ITipoMultaService
    {
        private readonly ITipoMultaRepository _tipoMultaRepository;
        private readonly ILogger<TipoMultaService> _logger;

        public TipoMultaService(ITipoMultaRepository tipoMultaRepository, ILogger<TipoMultaService> logger)
        {
            _tipoMultaRepository = tipoMultaRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<TipoMulta>> GetAllAsync()
        {
            try
            {
                return await _tipoMultaRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os tipos de multa");
                throw;
            }
        }

        public async Task<TipoMulta?> GetByIdAsync(int id)
        {
            try
            {
                return await _tipoMultaRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tipo de multa por ID: {Id}", id);
                throw;
            }
        }

        public async Task<TipoMulta?> GetByCodigoAsync(string codigo)
        {
            try
            {
                var tiposMulta = await _tipoMultaRepository.FindAsync(tm => tm.CodigoMulta == codigo);
                return tiposMulta.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tipo de multa por código: {Codigo}", codigo);
                throw;
            }
        }

        public async Task<TipoMulta> CreateAsync(TipoMulta tipoMulta)
        {
            try
            {
                // Validar dados obrigatórios
                if (string.IsNullOrWhiteSpace(tipoMulta.CodigoMulta))
                    throw new ArgumentException("Código da multa é obrigatório");

                if (string.IsNullOrWhiteSpace(tipoMulta.Gravidade))
                    throw new ArgumentException("Gravidade é obrigatória");

                // Validar tamanho dos campos
                if (tipoMulta.CodigoMulta.Length > 20)
                    throw new ArgumentException("Código da multa não pode ter mais de 20 caracteres");

                if (!string.IsNullOrEmpty(tipoMulta.Descricao) && tipoMulta.Descricao.Length > 255)
                    throw new ArgumentException("Descrição não pode ter mais de 255 caracteres");

                // Validar valor da multa
                if (tipoMulta.ValorMulta <= 0)
                    throw new ArgumentException("Valor da multa deve ser maior que zero");

                if (tipoMulta.ValorMulta > 999999.99m)
                    throw new ArgumentException("Valor da multa não pode ser maior que 999.999,99");

                // Validar pontos
                if (tipoMulta.Pontos < 0)
                    throw new ArgumentException("Pontos não podem ser negativos");

                if (tipoMulta.Pontos > 7)
                    throw new ArgumentException("Pontos não podem ser maiores que 7");

                // Validar gravidade
                var gravidadesValidas = new[] { "leve", "media", "grave", "gravissima" };
                if (!gravidadesValidas.Contains(tipoMulta.Gravidade.ToLower()))
                    throw new ArgumentException("Gravidade deve ser: leve, media, grave ou gravissima");

                // Validar se código já existe
                if (await CodigoExistsAsync(tipoMulta.CodigoMulta))
                    throw new InvalidOperationException("Código de multa já está em uso");

                // Normalizar dados
                tipoMulta.CodigoMulta = tipoMulta.CodigoMulta.Trim().ToUpper();
                tipoMulta.Descricao = tipoMulta.Descricao?.Trim();
                tipoMulta.Gravidade = tipoMulta.Gravidade.Trim().ToLower();

                await _tipoMultaRepository.AddAsync(tipoMulta);
                await _tipoMultaRepository.SaveChangesAsync();

                _logger.LogInformation("Tipo de multa criado com sucesso: {Codigo}", tipoMulta.CodigoMulta);
                return tipoMulta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tipo de multa: {Codigo}", tipoMulta.CodigoMulta);
                throw;
            }
        }

        public async Task<TipoMulta> UpdateAsync(TipoMulta tipoMulta)
        {
            try
            {
                var existingTipoMulta = await GetByIdAsync(tipoMulta.Id);
                if (existingTipoMulta == null)
                    throw new InvalidOperationException("Tipo de multa não encontrado");

                // Validar dados obrigatórios
                if (string.IsNullOrWhiteSpace(tipoMulta.CodigoMulta))
                    throw new ArgumentException("Código da multa é obrigatório");

                if (string.IsNullOrWhiteSpace(tipoMulta.Gravidade))
                    throw new ArgumentException("Gravidade é obrigatória");

                // Validar tamanho dos campos
                if (tipoMulta.CodigoMulta.Length > 20)
                    throw new ArgumentException("Código da multa não pode ter mais de 20 caracteres");

                if (!string.IsNullOrEmpty(tipoMulta.Descricao) && tipoMulta.Descricao.Length > 255)
                    throw new ArgumentException("Descrição não pode ter mais de 255 caracteres");

                // Validar valor da multa
                if (tipoMulta.ValorMulta <= 0)
                    throw new ArgumentException("Valor da multa deve ser maior que zero");

                if (tipoMulta.ValorMulta > 999999.99m)
                    throw new ArgumentException("Valor da multa não pode ser maior que 999.999,99");

                // Validar pontos
                if (tipoMulta.Pontos < 0)
                    throw new ArgumentException("Pontos não podem ser negativos");

                if (tipoMulta.Pontos > 7)
                    throw new ArgumentException("Pontos não podem ser maiores que 7");

                // Validar gravidade
                var gravidadesValidas = new[] { "leve", "media", "grave", "gravissima" };
                if (!gravidadesValidas.Contains(tipoMulta.Gravidade.ToLower()))
                    throw new ArgumentException("Gravidade deve ser: leve, media, grave ou gravissima");

                // Verificar se código foi alterado e se já existe
                if (tipoMulta.CodigoMulta != existingTipoMulta.CodigoMulta && await CodigoExistsAsync(tipoMulta.CodigoMulta))
                    throw new InvalidOperationException("Código de multa já está em uso");

                // Normalizar dados
                tipoMulta.CodigoMulta = tipoMulta.CodigoMulta.Trim().ToUpper();
                tipoMulta.Descricao = tipoMulta.Descricao?.Trim();
                tipoMulta.Gravidade = tipoMulta.Gravidade.Trim().ToLower();

                await _tipoMultaRepository.UpdateAsync(tipoMulta);
                await _tipoMultaRepository.SaveChangesAsync();

                _logger.LogInformation("Tipo de multa atualizado com sucesso: {Id}", tipoMulta.Id);
                return tipoMulta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tipo de multa: {Id}", tipoMulta.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var tipoMulta = await GetByIdAsync(id);
                if (tipoMulta == null)
                    throw new InvalidOperationException("Tipo de multa não encontrado");

                // Verificar se há multas associadas
                var multasAssociadas = tipoMulta.Multas?.Count ?? 0;
                if (multasAssociadas > 0)
                    throw new InvalidOperationException($"Não é possível deletar o tipo de multa. Existem {multasAssociadas} multa(s) associada(s)");

                await _tipoMultaRepository.DeleteAsync(tipoMulta);
                await _tipoMultaRepository.SaveChangesAsync();

                _logger.LogInformation("Tipo de multa deletado com sucesso: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar tipo de multa: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var tipoMulta = await GetByIdAsync(id);
                return tipoMulta != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do tipo de multa: {Id}", id);
                throw;
            }
        }

        public async Task<bool> CodigoExistsAsync(string codigo)
        {
            try
            {
                var tipoMulta = await GetByCodigoAsync(codigo);
                return tipoMulta != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do código: {Codigo}", codigo);
                throw;
            }
        }

        public async Task<IEnumerable<TipoMulta>> GetByGravidadeAsync(string gravidade)
        {
            try
            {
                var tiposMulta = await _tipoMultaRepository.FindAsync(tm => tm.Gravidade.ToLower() == gravidade.ToLower());
                return tiposMulta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar tipos de multa por gravidade: {Gravidade}", gravidade);
                throw;
            }
        }
    }
}
