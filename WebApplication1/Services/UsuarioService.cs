using WebApplication1.Entities;
using WebApplication1.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            try
            {
                return await _usuarioRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os usuários");
                throw;
            }
        }

        public async Task<Usuario?> GetByIdAsync(int id)
        {
            try
            {
                return await _usuarioRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por ID: {Id}", id);
                throw;
            }
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            try
            {
                var usuarios = await _usuarioRepository.FindAsync(u => u.Email == email);
                return usuarios.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar usuário por email: {Email}", email);
                throw;
            }
        }

        public async Task<Usuario?> AuthenticateAsync(string email, string senha)
        {
            try
            {
                var usuario = await GetByEmailAsync(email);
                if (usuario == null)
                    return null;

                var senhaHash = HashPassword(senha);
                if (usuario.Senha == senhaHash)
                    return usuario;

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao autenticar usuário: {Email}", email);
                throw;
            }
        }

        public async Task<Usuario> CreateAsync(Usuario usuario)
        {
            try
            {
                // Validar dados obrigatórios
                if (string.IsNullOrWhiteSpace(usuario.Nome))
                    throw new ArgumentException("Nome do usuário é obrigatório");

                if (string.IsNullOrWhiteSpace(usuario.Email))
                    throw new ArgumentException("Email do usuário é obrigatório");

                if (string.IsNullOrWhiteSpace(usuario.Senha))
                    throw new ArgumentException("Senha do usuário é obrigatória");

                if (string.IsNullOrWhiteSpace(usuario.Tipo))
                    throw new ArgumentException("Tipo do usuário é obrigatório");

                // Validar tamanho dos campos
                if (usuario.Nome.Length > 100)
                    throw new ArgumentException("Nome não pode ter mais de 100 caracteres");

                if (usuario.Email.Length > 100)
                    throw new ArgumentException("Email não pode ter mais de 100 caracteres");

                if (usuario.Senha.Length > 100)
                    throw new ArgumentException("Senha não pode ter mais de 100 caracteres");

                if (!string.IsNullOrEmpty(usuario.CPF) && usuario.CPF.Length > 14)
                    throw new ArgumentException("CPF não pode ter mais de 14 caracteres");

                // Validar formato do email
                if (!ValidarEmail(usuario.Email))
                    throw new ArgumentException("Formato de email inválido");

                // Validar tipo de usuário
                var tiposValidos = new[] { "agente", "supervisor", "administrador" };
                if (!tiposValidos.Contains(usuario.Tipo.ToLower()))
                    throw new ArgumentException("Tipo deve ser: agente, supervisor ou administrador");

                // Validar se email já existe
                if (await EmailExistsAsync(usuario.Email))
                    throw new InvalidOperationException("Email já está em uso");

                // Validar força da senha
                if (!ValidarSenha(usuario.Senha))
                    throw new ArgumentException("Senha deve ter pelo menos 6 caracteres");

                // Normalizar dados
                usuario.Nome = usuario.Nome.Trim();
                usuario.Email = usuario.Email.Trim().ToLower();
                usuario.CPF = usuario.CPF?.Trim();
                usuario.Tipo = usuario.Tipo.Trim().ToLower();

                // Hash da senha
                usuario.Senha = HashPassword(usuario.Senha);
                usuario.DataCriacao = DateTime.UtcNow;

                await _usuarioRepository.AddAsync(usuario);
                await _usuarioRepository.SaveChangesAsync();

                _logger.LogInformation("Usuário criado com sucesso: {Email}", usuario.Email);
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar usuário: {Email}", usuario.Email);
                throw;
            }
        }

        public async Task<Usuario> UpdateAsync(Usuario usuario)
        {
            try
            {
                var existingUsuario = await GetByIdAsync(usuario.Id);
                if (existingUsuario == null)
                    throw new InvalidOperationException("Usuário não encontrado");

                // Validar dados obrigatórios
                if (string.IsNullOrWhiteSpace(usuario.Nome))
                    throw new ArgumentException("Nome do usuário é obrigatório");

                if (string.IsNullOrWhiteSpace(usuario.Email))
                    throw new ArgumentException("Email do usuário é obrigatório");

                if (string.IsNullOrWhiteSpace(usuario.Tipo))
                    throw new ArgumentException("Tipo do usuário é obrigatório");

                // Validar tamanho dos campos
                if (usuario.Nome.Length > 100)
                    throw new ArgumentException("Nome não pode ter mais de 100 caracteres");

                if (usuario.Email.Length > 100)
                    throw new ArgumentException("Email não pode ter mais de 100 caracteres");

                if (!string.IsNullOrEmpty(usuario.CPF) && usuario.CPF.Length > 14)
                    throw new ArgumentException("CPF não pode ter mais de 14 caracteres");

                // Validar formato do email
                if (!ValidarEmail(usuario.Email))
                    throw new ArgumentException("Formato de email inválido");

                // Validar tipo de usuário
                var tiposValidos = new[] { "agente", "supervisor", "administrador" };
                if (!tiposValidos.Contains(usuario.Tipo.ToLower()))
                    throw new ArgumentException("Tipo deve ser: agente, supervisor ou administrador");

                // Verificar se email foi alterado e se já existe
                if (usuario.Email.ToLower() != existingUsuario.Email.ToLower() && await EmailExistsAsync(usuario.Email))
                    throw new InvalidOperationException("Email já está em uso");

                // Normalizar dados
                usuario.Nome = usuario.Nome.Trim();
                usuario.Email = usuario.Email.Trim().ToLower();
                usuario.CPF = usuario.CPF?.Trim();
                usuario.Tipo = usuario.Tipo.Trim().ToLower();

                // Se a senha foi alterada, fazer hash
                if (!string.IsNullOrEmpty(usuario.Senha) && usuario.Senha != existingUsuario.Senha)
                {
                    if (!ValidarSenha(usuario.Senha))
                        throw new ArgumentException("Senha deve ter pelo menos 6 caracteres");
                    usuario.Senha = HashPassword(usuario.Senha);
                }
                else
                {
                    usuario.Senha = existingUsuario.Senha;
                }

                await _usuarioRepository.UpdateAsync(usuario);
                await _usuarioRepository.SaveChangesAsync();

                _logger.LogInformation("Usuário atualizado com sucesso: {Id}", usuario.Id);
                return usuario;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar usuário: {Id}", usuario.Id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                var usuario = await GetByIdAsync(id);
                if (usuario == null)
                    throw new InvalidOperationException("Usuário não encontrado");

                // Verificar se há multas associadas
                var multasAssociadas = usuario.Multas?.Count ?? 0;
                if (multasAssociadas > 0)
                    throw new InvalidOperationException($"Não é possível deletar o usuário. Existem {multasAssociadas} multa(s) associada(s)");

                await _usuarioRepository.DeleteAsync(usuario);
                await _usuarioRepository.SaveChangesAsync();

                _logger.LogInformation("Usuário deletado com sucesso: {Id}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar usuário: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var usuario = await GetByIdAsync(id);
                return usuario != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do usuário: {Id}", id);
                throw;
            }
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            try
            {
                var usuario = await GetByEmailAsync(email);
                return usuario != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao verificar existência do email: {Email}", email);
                throw;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool ValidarEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidarSenha(string senha)
        {
            return !string.IsNullOrWhiteSpace(senha) && senha.Length >= 6;
        }
    }
}
