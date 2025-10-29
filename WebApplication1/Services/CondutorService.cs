using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Services
{
    public class CondutorService : ICondutorService
{
    private readonly ICondutorRepository _condutorRepository;
        private readonly ILogger<CondutorService> _logger;

        public CondutorService(ICondutorRepository condutorRepository, ILogger<CondutorService> logger)
    {
        _condutorRepository = condutorRepository;
        _logger = logger;
    }

        public async Task<IEnumerable<Condutor>> GetAllAsync()
        {
            try
            {
                return await _condutorRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os condutores");
                throw;
            }
        }

        public async Task<Condutor?> GetByIdAsync(int id)
        {
            try
            {
                return await _condutorRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar condutor por ID: {Id}", id);
                throw;
            }
        }

        public async Task<Condutor?> GetByCPFAsync(string cpf)
        {
            try
            {
                var condutores = await _condutorRepository.FindAsync(c => c.CPF == cpf);
                return condutores.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar condutor por CPF: {CPF}", cpf);
                throw;
            }
        }

        public async Task<Condutor?> GetByCNHAsync(string cnh)
        {
            try
            {
                var condutores = await _condutorRepository.FindAsync(c => c.CNH == cnh);
                return condutores.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar condutor por CNH: {CNH}", cnh);
                throw;
            }
        }

        public async Task<Condutor> CreateAsync(Condutor condutor)
        {
            try
            {
                // Validar dados obrigatórios
                if (string.IsNullOrWhiteSpace(condutor.Nome))
                    throw new ArgumentException("Nome do condutor é obrigatório");

                if (string.IsNullOrWhiteSpace(condutor.CPF))
                    throw new ArgumentException("CPF do condutor é obrigatório");

                if (string.IsNullOrWhiteSpace(condutor.CNH))
                    throw new ArgumentException("CNH do condutor é obrigatória");

                // Validar tamanho dos campos
                if (condutor.Nome.Length > 100)
                    throw new ArgumentException("Nome do condutor não pode ter mais de 100 caracteres");

                if (condutor.CPF.Length > 14)
                    throw new ArgumentException("CPF não pode ter mais de 14 caracteres");

                if (condutor.CNH.Length > 20)
                    throw new ArgumentException("CNH não pode ter mais de 20 caracteres");

                if (!string.IsNullOrEmpty(condutor.CPF_UF) && condutor.CPF_UF.Length > 2)
                    throw new ArgumentException("CPF_UF não pode ter mais de 2 caracteres");

                if (!string.IsNullOrEmpty(condutor.CategoriaCNH) && condutor.CategoriaCNH.Length > 5)
                    throw new ArgumentException("Categoria CNH não pode ter mais de 5 caracteres");

                // Validar se CPF já existe
                if (await CPFExistsAsync(condutor.CPF))
                    throw new InvalidOperationException("CPF já está em uso");

                // Validar se CNH já existe
                if (await CNHExistsAsync(condutor.CNH))
                    throw new InvalidOperationException("CNH já está em uso");

                // Validar CPF
                if (!ValidarCPF(condutor.CPF))
                    throw new InvalidOperationException("CPF inválido");

                // Normalizar dados
                condutor.Nome = condutor.Nome.Trim();
                condutor.CPF = condutor.CPF.Trim();
                condutor.CNH = condutor.CNH.Trim();
                condutor.CPF_UF = condutor.CPF_UF?.Trim().ToUpper();
                condutor.CategoriaCNH = condutor.CategoriaCNH?.Trim().ToUpper();

                await _condutorRepository.AddAsync(condutor);
                await _condutorRepository.SaveChangesAsync();

                _logger.LogInformation("Condutor criado com sucesso: {Nome}", condutor.Nome);
                return condutor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar condutor: {Nome}", condutor.Nome);
                throw;
            }
        }

        public async Task<Condutor> UpdateAsync(Condutor condutor)
        {
            try
            {
                var existingCondutor = await GetByIdAsync(condutor.Id);
                if (existingCondutor == null)
                    throw new InvalidOperationException("Condutor não encontrado");

                // Validar dados obrigatórios
                if (string.IsNullOrWhiteSpace(condutor.Nome))
                    throw new ArgumentException("Nome do condutor é obrigatório");

                if (string.IsNullOrWhiteSpace(condutor.CPF))
                    throw new ArgumentException("CPF do condutor é obrigatório");

                if (string.IsNullOrWhiteSpace(condutor.CNH))
                    throw new ArgumentException("CNH do condutor é obrigatória");

                // Validar tamanho dos campos
                if (condutor.Nome.Length > 100)
                    throw new ArgumentException("Nome do condutor não pode ter mais de 100 caracteres");

                if (condutor.CPF.Length > 14)
                    throw new ArgumentException("CPF não pode ter mais de 14 caracteres");

                if (condutor.CNH.Length > 20)
                    throw new ArgumentException("CNH não pode ter mais de 20 caracteres");

                if (!string.IsNullOrEmpty(condutor.CPF_UF) && condutor.CPF_UF.Length > 2)
                    throw new ArgumentException("CPF_UF não pode ter mais de 2 caracteres");

                if (!string.IsNullOrEmpty(condutor.CategoriaCNH) && condutor.CategoriaCNH.Length > 5)
                    throw new ArgumentException("Categoria CNH não pode ter mais de 5 caracteres");

                // Verificar se CPF foi alterado e se já existe
                if (condutor.CPF != existingCondutor.CPF && await CPFExistsAsync(condutor.CPF))
                    throw new InvalidOperationException("CPF já está em uso");

                // Verificar se CNH foi alterada e se já existe
                if (condutor.CNH != existingCondutor.CNH && await CNHExistsAsync(condutor.CNH))
                    throw new InvalidOperationException("CNH já está em uso");

                // Validar CPF
                if (!ValidarCPF(condutor.CPF))
                    throw new InvalidOperationException("CPF inválido");

                // Normalizar dados
                condutor.Nome = condutor.Nome.Trim();
                condutor.CPF = condutor.CPF.Trim();
                condutor.CNH = condutor.CNH.Trim();
                condutor.CPF_UF = condutor.CPF_UF?.Trim().ToUpper();
                condutor.CategoriaCNH = condutor.CategoriaCNH?.Trim().ToUpper();

                await _condutorRepository.UpdateAsync(condutor);
                await _condutorRepository.SaveChangesAsync();

                _logger.LogInformation("Condutor atualizado com sucesso: {Id}", condutor.Id);
                return condutor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar condutor: {Id}", condutor.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var condutor = await GetByIdAsync(id);
                if (condutor == null)
                    throw new InvalidOperationException("Condutor não encontrado");

                // Verificar se há multas associadas
                var multasAssociadas = condutor.Multas?.Count ?? 0;
                if (multasAssociadas > 0)
                    throw new InvalidOperationException($"Não é possível deletar o condutor. Existem {multasAssociadas} multa(s) associada(s)");

                await _condutorRepository.DeleteAsync(condutor);
                await _condutorRepository.SaveChangesAsync();

                _logger.LogInformation("Condutor deletado com sucesso: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar condutor: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var condutor = await GetByIdAsync(id);
                return condutor != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do condutor: {Id}", id);
                throw;
            }
        }

        public async Task<bool> CPFExistsAsync(string cpf)
        {
            try
            {
                var condutor = await GetByCPFAsync(cpf);
                return condutor != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do CPF: {CPF}", cpf);
                throw;
            }
        }

        public async Task<bool> CNHExistsAsync(string cnh)
        {
            try
            {
                var condutor = await GetByCNHAsync(cnh);
                return condutor != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência da CNH: {CNH}", cnh);
                throw;
            }
        }

        private bool ValidarCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return false;

            // Remove caracteres não numéricos
            cpf = cpf.Replace(".", "").Replace("-", "").Replace(" ", "");

            // Verifica se tem 11 dígitos
            if (cpf.Length != 11)
                return false;

            // Verifica se todos os dígitos são iguais
            if (cpf.All(c => c == cpf[0]))
                return false;

            // Validação do CPF
            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCpf = cpf.Substring(0, 9);
            int soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            string digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;

            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;

            digito = digito + resto.ToString();

            return cpf.EndsWith(digito);
        }
    }
}
