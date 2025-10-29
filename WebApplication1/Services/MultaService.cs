using WebApplication1.Entities;
using WebApplication1.Interfaces;
using System.Text.Json;

namespace WebApplication1.Services
{
    public class MultaService : IMultaService
    {
        private readonly IMultaRepository _multaRepository;
        private readonly IVeiculoService _veiculoService;
        private readonly ICondutorService _condutorService;
        private readonly ITipoMultaService _tipoMultaService;
        private readonly IUsuarioService _usuarioService;
        private readonly IAIProcessingService _aiProcessingService;
        private readonly ILogger<MultaService> _logger;

        public MultaService(
            IMultaRepository multaRepository,
            IVeiculoService veiculoService,
            ICondutorService condutorService,
            ITipoMultaService tipoMultaService,
            IUsuarioService usuarioService,
            IAIProcessingService aiProcessingService,
            ILogger<MultaService> logger)
        {
            _multaRepository = multaRepository;
            _veiculoService = veiculoService;
            _condutorService = condutorService;
            _tipoMultaService = tipoMultaService;
            _usuarioService = usuarioService;
            _aiProcessingService = aiProcessingService;
            _logger = logger;
        }

        public async Task<IEnumerable<Multa>> GetAllAsync()
        {
            try
            {
                return await _multaRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as multas");
                throw;
            }
        }

        public async Task<Multa?> GetByIdAsync(int id)
        {
            try
            {
                return await _multaRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar multa por ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Multa>> GetByVeiculoIdAsync(int veiculoId)
        {
            try
            {
                var multas = await _multaRepository.FindAsync(m => m.VeiculoId == veiculoId);
                return multas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar multas por veículo ID: {VeiculoId}", veiculoId);
                throw;
            }
        }

        public async Task<IEnumerable<Multa>> GetByCondutorIdAsync(int condutorId)
        {
            try
            {
                var multas = await _multaRepository.FindAsync(m => m.CondutorId == condutorId);
                return multas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar multas por condutor ID: {CondutorId}", condutorId);
                throw;
            }
        }

        public async Task<IEnumerable<Multa>> GetByUsuarioIdAsync(int usuarioId)
        {
            try
            {
                var multas = await _multaRepository.FindAsync(m => m.UsuarioId == usuarioId);
                return multas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar multas por usuário ID: {UsuarioId}", usuarioId);
                throw;
            }
        }

        public async Task<IEnumerable<Multa>> GetByTipoMultaIdAsync(int tipoMultaId)
        {
            try
            {
                var multas = await _multaRepository.FindAsync(m => m.TipoMultaId == tipoMultaId);
                return multas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar multas por tipo multa ID: {TipoMultaId}", tipoMultaId);
                throw;
            }
        }

        public async Task<IEnumerable<Multa>> GetByDateRangeAsync(DateTime dataInicio, DateTime dataFim)
        {
            try
            {
                var multas = await _multaRepository.FindAsync(m => m.DataHora >= dataInicio && m.DataHora <= dataFim);
                return multas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar multas por período: {DataInicio} - {DataFim}", dataInicio, dataFim);
                throw;
            }
        }

        public async Task<Multa> CreateAsync(Multa multa)
        {
            try
            {
                // Validar dados obrigatórios
                if (multa.VeiculoId <= 0)
                    throw new ArgumentException("ID do veículo é obrigatório");

                if (multa.UsuarioId <= 0)
                    throw new ArgumentException("ID do usuário é obrigatório");

                if (multa.TipoMultaId <= 0)
                    throw new ArgumentException("ID do tipo de multa é obrigatório");

                // Validar tamanho dos campos opcionais
                if (!string.IsNullOrEmpty(multa.Endereco) && multa.Endereco.Length > 255)
                    throw new ArgumentException("Endereço não pode ter mais de 255 caracteres");

                if (!string.IsNullOrEmpty(multa.Descricao) && multa.Descricao.Length > 255)
                    throw new ArgumentException("Descrição não pode ter mais de 255 caracteres");

                // Validar se veículo existe
                if (!await _veiculoService.ExistsAsync(multa.VeiculoId))
                    throw new InvalidOperationException("Veículo não encontrado");

                // Validar se usuário existe
                if (!await _usuarioService.ExistsAsync(multa.UsuarioId))
                    throw new InvalidOperationException("Usuário não encontrado");

                // Validar se tipo de multa existe
                if (!await _tipoMultaService.ExistsAsync(multa.TipoMultaId))
                    throw new InvalidOperationException("Tipo de multa não encontrado");

                // Validar se condutor existe (se informado)
                if (multa.CondutorId.HasValue && !await _condutorService.ExistsAsync(multa.CondutorId.Value))
                    throw new InvalidOperationException("Condutor não encontrado");

                // Validar data/hora
                if (multa.DataHora > DateTime.Now)
                    throw new ArgumentException("Data e hora não podem ser futuras");

                if (multa.DataHora < DateTime.Now.AddYears(-10))
                    throw new ArgumentException("Data e hora não podem ser muito antigas (mais de 10 anos)");

                // Normalizar dados
                multa.Endereco = multa.Endereco?.Trim();
                multa.Descricao = multa.Descricao?.Trim();

                await _multaRepository.AddAsync(multa);
                await _multaRepository.SaveChangesAsync();

                _logger.LogInformation("Multa criada com sucesso: {Id}", multa.Id);
                return multa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar multa");
                throw;
            }
        }

        public async Task<Multa> UpdateAsync(Multa multa)
        {
            try
            {
                var existingMulta = await GetByIdAsync(multa.Id);
                if (existingMulta == null)
                    throw new InvalidOperationException("Multa não encontrada");

                // Validar dados obrigatórios
                if (multa.VeiculoId <= 0)
                    throw new ArgumentException("ID do veículo é obrigatório");

                if (multa.UsuarioId <= 0)
                    throw new ArgumentException("ID do usuário é obrigatório");

                if (multa.TipoMultaId <= 0)
                    throw new ArgumentException("ID do tipo de multa é obrigatório");

                // Validar tamanho dos campos opcionais
                if (!string.IsNullOrEmpty(multa.Endereco) && multa.Endereco.Length > 255)
                    throw new ArgumentException("Endereço não pode ter mais de 255 caracteres");

                if (!string.IsNullOrEmpty(multa.Descricao) && multa.Descricao.Length > 255)
                    throw new ArgumentException("Descrição não pode ter mais de 255 caracteres");

                // Validar se veículo existe
                if (!await _veiculoService.ExistsAsync(multa.VeiculoId))
                    throw new InvalidOperationException("Veículo não encontrado");

                // Validar se usuário existe
                if (!await _usuarioService.ExistsAsync(multa.UsuarioId))
                    throw new InvalidOperationException("Usuário não encontrado");

                // Validar se tipo de multa existe
                if (!await _tipoMultaService.ExistsAsync(multa.TipoMultaId))
                    throw new InvalidOperationException("Tipo de multa não encontrado");

                // Validar se condutor existe (se informado)
                if (multa.CondutorId.HasValue && !await _condutorService.ExistsAsync(multa.CondutorId.Value))
                    throw new InvalidOperationException("Condutor não encontrado");

                // Validar data/hora
                if (multa.DataHora > DateTime.Now)
                    throw new ArgumentException("Data e hora não podem ser futuras");

                if (multa.DataHora < DateTime.Now.AddYears(-10))
                    throw new ArgumentException("Data e hora não podem ser muito antigas (mais de 10 anos)");

                // Normalizar dados
                multa.Endereco = multa.Endereco?.Trim();
                multa.Descricao = multa.Descricao?.Trim();

                await _multaRepository.UpdateAsync(multa);
                await _multaRepository.SaveChangesAsync();

                _logger.LogInformation("Multa atualizada com sucesso: {Id}", multa.Id);
                return multa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar multa: {Id}", multa.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var multa = await GetByIdAsync(id);
                if (multa == null)
                    throw new InvalidOperationException("Multa não encontrada");

                await _multaRepository.DeleteAsync(multa);
                await _multaRepository.SaveChangesAsync();

                _logger.LogInformation("Multa deletada com sucesso: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar multa: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var multa = await GetByIdAsync(id);
                return multa != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência da multa: {Id}", id);
                throw;
            }
        }

        public async Task<Multa> ProcessarMultaFromAIAsync(string jsonData, int usuarioId)
        {
            try
            {
                _logger.LogInformation("Iniciando processamento de multa via IA para usuário: {UsuarioId}", usuarioId);

                // Extrair dados da IA
                var dadosMulta = await _aiProcessingService.ExtrairDadosMultaAsync(jsonData);

                // Buscar ou criar veículo
                var veiculo = await _veiculoService.GetByPlacaAsync(dadosMulta.PlacaVeiculo!);
                if (veiculo == null)
                {
                    veiculo = new Veiculo
                    {
                        Placa = dadosMulta.PlacaVeiculo!,
                        Marca = dadosMulta.MarcaVeiculo,
                        Modelo = dadosMulta.ModeloVeiculo,
                        Tipo = dadosMulta.TipoVeiculo,
                        Proprietario = dadosMulta.ProprietarioVeiculo
                    };
                    veiculo = await _veiculoService.CreateAsync(veiculo);
                }

                // Buscar ou criar condutor
                var condutor = await _condutorService.GetByCPFAsync(dadosMulta.CPFCondutor!);
                if (condutor == null)
                {
                    condutor = new Condutor
                    {
                        Nome = dadosMulta.NomeCondutor!,
                        CPF = dadosMulta.CPFCondutor!,
                        CNH = dadosMulta.CNHCondutor!,
                        CategoriaCNH = dadosMulta.CategoriaCNH,
                        CPF_UF = dadosMulta.CPF_UF
                    };
                    condutor = await _condutorService.CreateAsync(condutor);
                }

                // Buscar tipo de multa
                var tipoMulta = await _tipoMultaService.GetByCodigoAsync(dadosMulta.CodigoMulta!);
                if (tipoMulta == null)
                    throw new InvalidOperationException($"Tipo de multa com código {dadosMulta.CodigoMulta} não encontrado");

                // Criar multa
                var multa = new Multa
                {
                    VeiculoId = veiculo.Id,
                    UsuarioId = usuarioId,
                    CondutorId = condutor.Id,
                    DataHora = dadosMulta.DataHora!.Value,
                    Endereco = dadosMulta.Endereco,
                    Descricao = dadosMulta.Descricao,
                    TipoMultaId = tipoMulta.Id
                };

                multa = await CreateAsync(multa);

                _logger.LogInformation("Multa processada via IA com sucesso: {Id}", multa.Id);
                return multa;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar multa via IA para usuário: {UsuarioId}", usuarioId);
                throw;
            }
        }
    }
}
