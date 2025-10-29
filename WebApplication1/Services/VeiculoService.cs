using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly ILogger<VeiculoService> _logger;

        public VeiculoService(IVeiculoRepository veiculoRepository, ILogger<VeiculoService> logger)
        {
            _veiculoRepository = veiculoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Veiculo>> GetAllAsync()
        {
            try
            {
                return await _veiculoRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os veículos");
                throw;
            }
        }

        public async Task<Veiculo?> GetByIdAsync(int id)
        {
            try
            {
                return await _veiculoRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar veículo por ID: {Id}", id);
                throw;
            }
        }

        public async Task<Veiculo?> GetByPlacaAsync(string placa)
        {
            try
            {
                var veiculos = await _veiculoRepository.FindAsync(v => v.Placa.ToUpper() == placa.ToUpper());
                return veiculos.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar veículo por placa: {Placa}", placa);
                throw;
            }
        }

        public async Task<Veiculo> CreateAsync(Veiculo veiculo)
        {
            try
            {
                // Validar dados obrigatórios
                if (string.IsNullOrWhiteSpace(veiculo.Placa))
                    throw new ArgumentException("Placa do veículo é obrigatória");

                // Validar tamanho dos campos
                if (veiculo.Placa.Length > 10)
                    throw new ArgumentException("Placa não pode ter mais de 10 caracteres");

                if (!string.IsNullOrEmpty(veiculo.Marca) && veiculo.Marca.Length > 50)
                    throw new ArgumentException("Marca não pode ter mais de 50 caracteres");

                if (!string.IsNullOrEmpty(veiculo.Modelo) && veiculo.Modelo.Length > 50)
                    throw new ArgumentException("Modelo não pode ter mais de 50 caracteres");

                if (!string.IsNullOrEmpty(veiculo.Tipo) && veiculo.Tipo.Length > 30)
                    throw new ArgumentException("Tipo não pode ter mais de 30 caracteres");

                if (!string.IsNullOrEmpty(veiculo.Proprietario) && veiculo.Proprietario.Length > 100)
                    throw new ArgumentException("Proprietário não pode ter mais de 100 caracteres");

                // Validar se placa já existe
                if (await PlacaExistsAsync(veiculo.Placa))
                    throw new InvalidOperationException("Placa já está em uso");

                // Validar formato da placa
                if (!ValidarPlaca(veiculo.Placa))
                    throw new InvalidOperationException("Formato de placa inválido");

                // Normalizar dados
                veiculo.Placa = veiculo.Placa.Trim().ToUpper();
                veiculo.Marca = veiculo.Marca?.Trim();
                veiculo.Modelo = veiculo.Modelo?.Trim();
                veiculo.Tipo = veiculo.Tipo?.Trim();
                veiculo.Proprietario = veiculo.Proprietario?.Trim();

                await _veiculoRepository.AddAsync(veiculo);
                await _veiculoRepository.SaveChangesAsync();

                _logger.LogInformation("Veículo criado com sucesso: {Placa}", veiculo.Placa);
                return veiculo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar veículo: {Placa}", veiculo.Placa);
                throw;
            }
        }

        public async Task<Veiculo> UpdateAsync(Veiculo veiculo)
        {
            try
            {
                var existingVeiculo = await GetByIdAsync(veiculo.Id);
                if (existingVeiculo == null)
                    throw new InvalidOperationException("Veículo não encontrado");

                // Validar dados obrigatórios
                if (string.IsNullOrWhiteSpace(veiculo.Placa))
                    throw new ArgumentException("Placa do veículo é obrigatória");

                // Validar tamanho dos campos
                if (veiculo.Placa.Length > 10)
                    throw new ArgumentException("Placa não pode ter mais de 10 caracteres");

                if (!string.IsNullOrEmpty(veiculo.Marca) && veiculo.Marca.Length > 50)
                    throw new ArgumentException("Marca não pode ter mais de 50 caracteres");

                if (!string.IsNullOrEmpty(veiculo.Modelo) && veiculo.Modelo.Length > 50)
                    throw new ArgumentException("Modelo não pode ter mais de 50 caracteres");

                if (!string.IsNullOrEmpty(veiculo.Tipo) && veiculo.Tipo.Length > 30)
                    throw new ArgumentException("Tipo não pode ter mais de 30 caracteres");

                if (!string.IsNullOrEmpty(veiculo.Proprietario) && veiculo.Proprietario.Length > 100)
                    throw new ArgumentException("Proprietário não pode ter mais de 100 caracteres");

                // Verificar se placa foi alterada e se já existe
                if (veiculo.Placa.ToUpper() != existingVeiculo.Placa.ToUpper() && await PlacaExistsAsync(veiculo.Placa))
                    throw new InvalidOperationException("Placa já está em uso");

                // Validar formato da placa
                if (!ValidarPlaca(veiculo.Placa))
                    throw new InvalidOperationException("Formato de placa inválido");

                // Normalizar dados
                veiculo.Placa = veiculo.Placa.Trim().ToUpper();
                veiculo.Marca = veiculo.Marca?.Trim();
                veiculo.Modelo = veiculo.Modelo?.Trim();
                veiculo.Tipo = veiculo.Tipo?.Trim();
                veiculo.Proprietario = veiculo.Proprietario?.Trim();

                await _veiculoRepository.UpdateAsync(veiculo);
                await _veiculoRepository.SaveChangesAsync();

                _logger.LogInformation("Veículo atualizado com sucesso: {Id}", veiculo.Id);
                return veiculo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar veículo: {Id}", veiculo.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var veiculo = await GetByIdAsync(id);
                if (veiculo == null)
                    throw new InvalidOperationException("Veículo não encontrado");

                // Verificar se há multas associadas
                var multasAssociadas = veiculo.Multas?.Count ?? 0;
                if (multasAssociadas > 0)
                    throw new InvalidOperationException($"Não é possível deletar o veículo. Existem {multasAssociadas} multa(s) associada(s)");

                await _veiculoRepository.DeleteAsync(veiculo);
                await _veiculoRepository.SaveChangesAsync();

                _logger.LogInformation("Veículo deletado com sucesso: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar veículo: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var veiculo = await GetByIdAsync(id);
                return veiculo != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do veículo: {Id}", id);
                throw;
            }
        }

        public async Task<bool> PlacaExistsAsync(string placa)
        {
            try
            {
                var veiculo = await GetByPlacaAsync(placa);
                return veiculo != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência da placa: {Placa}", placa);
                throw;
            }
        }

        public async Task<IEnumerable<Veiculo>> SearchByPlacaAsync(string placa)
        {
            try
            {
                var veiculos = await _veiculoRepository.FindAsync(v => v.Placa.ToUpper().Contains(placa.ToUpper()));
                return veiculos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar veículos por placa: {Placa}", placa);
                throw;
            }
        }

        private bool ValidarPlaca(string placa)
        {
            if (string.IsNullOrEmpty(placa))
                return false;

            // Remove espaços e converte para maiúscula
            placa = placa.Replace(" ", "").ToUpper();

            // Validação para placas antigas (ABC-1234) ou Mercosul (ABC1D23)
            if (placa.Length == 7)
            {
                // Placa Mercosul: ABC1D23
                if (char.IsLetter(placa[0]) && char.IsLetter(placa[1]) && char.IsLetter(placa[2]) &&
                    char.IsDigit(placa[3]) && char.IsLetter(placa[4]) && char.IsDigit(placa[5]) && char.IsDigit(placa[6]))
                {
                    return true;
                }

                // Placa antiga: ABC1234
                if (char.IsLetter(placa[0]) && char.IsLetter(placa[1]) && char.IsLetter(placa[2]) &&
                    char.IsDigit(placa[3]) && char.IsDigit(placa[4]) && char.IsDigit(placa[5]) && char.IsDigit(placa[6]))
                {
                    return true;
                }
            }

            // Placa com hífen: ABC-1234
            if (placa.Length == 8 && placa[3] == '-')
            {
                var partes = placa.Split('-');
                if (partes.Length == 2 && partes[0].Length == 3 && partes[1].Length == 4)
                {
                    return char.IsLetter(partes[0][0]) && char.IsLetter(partes[0][1]) && char.IsLetter(partes[0][2]) &&
                           char.IsDigit(partes[1][0]) && char.IsDigit(partes[1][1]) && char.IsDigit(partes[1][2]) && char.IsDigit(partes[1][3]);
                }
            }

            return false;
        }
    }
}
