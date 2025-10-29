using WebApplication1.Interfaces;
using System.Text.Json;

namespace WebApplication1.Services
{
    public class AIProcessingService : IAIProcessingService
    {
        private readonly ILogger<AIProcessingService> _logger;

        public AIProcessingService(ILogger<AIProcessingService> logger)
        {
            _logger = logger;
        }

        public async Task<string> ProcessarImagemMultaAsync(byte[] imagemBytes)
        {
            try
            {
                _logger.LogInformation("Iniciando processamento de imagem de multa");

                // Aqui seria integrado com o serviço de IA real (OpenAI, Azure Cognitive Services, etc.)
                // Por enquanto, retornamos um JSON de exemplo
                var dadosExemplo = new MultaData
                {
                    PlacaVeiculo = "ABC-1234",
                    NomeCondutor = "João Silva Santos",
                    CPFCondutor = "123.456.789-00",
                    CNHCondutor = "12345678901",
                    DataHora = DateTime.Now,
                    Endereco = "Rua das Flores, 123 - Centro",
                    Descricao = "Estacionamento em local proibido",
                    CodigoMulta = "501-1",
                    MarcaVeiculo = "Toyota",
                    ModeloVeiculo = "Corolla",
                    TipoVeiculo = "Automóvel",
                    ProprietarioVeiculo = "Maria Silva Santos",
                    CategoriaCNH = "B",
                    CPF_UF = "SP"
                };

                var jsonResult = await ConverterParaJsonAsync(dadosExemplo);
                
                _logger.LogInformation("Processamento de imagem concluído com sucesso");
                return jsonResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar imagem de multa");
                throw;
            }
        }

        public async Task<MultaData> ExtrairDadosMultaAsync(string jsonData)
        {
            try
            {
                _logger.LogInformation("Iniciando extração de dados da multa");

                var dadosMulta = JsonSerializer.Deserialize<MultaData>(jsonData, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (dadosMulta == null)
                    throw new InvalidOperationException("Falha ao deserializar dados da multa");

                // Validar dados extraídos
                await ValidarDadosMultaAsync(dadosMulta);

                _logger.LogInformation("Extração de dados concluída com sucesso");
                return dadosMulta;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao extrair dados da multa");
                throw;
            }
        }

        public async Task<bool> ValidarDadosMultaAsync(MultaData dados)
        {
            try
            {
                _logger.LogInformation("Iniciando validação dos dados da multa");

                var erros = new List<string>();

                // Validar placa
                if (string.IsNullOrEmpty(dados.PlacaVeiculo))
                    erros.Add("Placa do veículo é obrigatória");
                else if (!ValidarPlaca(dados.PlacaVeiculo))
                    erros.Add("Formato de placa inválido");

                // Validar CPF do condutor
                if (string.IsNullOrEmpty(dados.CPFCondutor))
                    erros.Add("CPF do condutor é obrigatório");
                else if (!ValidarCPF(dados.CPFCondutor))
                    erros.Add("CPF do condutor inválido");

                // Validar CNH
                if (string.IsNullOrEmpty(dados.CNHCondutor))
                    erros.Add("CNH do condutor é obrigatória");

                // Validar nome do condutor
                if (string.IsNullOrEmpty(dados.NomeCondutor))
                    erros.Add("Nome do condutor é obrigatório");

                // Validar código da multa
                if (string.IsNullOrEmpty(dados.CodigoMulta))
                    erros.Add("Código da multa é obrigatório");

                // Validar data/hora
                if (!dados.DataHora.HasValue)
                    erros.Add("Data e hora são obrigatórias");
                else if (dados.DataHora.Value > DateTime.Now)
                    erros.Add("Data e hora não podem ser futuras");

                // Validar endereço
                if (string.IsNullOrEmpty(dados.Endereco))
                    erros.Add("Endereço é obrigatório");

                // Validar descrição
                if (string.IsNullOrEmpty(dados.Descricao))
                    erros.Add("Descrição da multa é obrigatória");

                if (erros.Any())
                {
                    var mensagemErro = string.Join("; ", erros);
                    _logger.LogWarning("Dados da multa inválidos: {Erros}", mensagemErro);
                    throw new InvalidOperationException($"Dados inválidos: {mensagemErro}");
                }

                _logger.LogInformation("Validação dos dados concluída com sucesso");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao validar dados da multa");
                throw;
            }
        }

        public async Task<string> ConverterParaJsonAsync(MultaData dados)
        {
            try
            {
                _logger.LogInformation("Convertendo dados para JSON");

                var opcoes = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                var json = JsonSerializer.Serialize(dados, opcoes);
                
                _logger.LogInformation("Conversão para JSON concluída com sucesso");
                return json;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao converter dados para JSON");
                throw;
            }
        }

        private bool ValidarPlaca(string placa)
        {
            if (string.IsNullOrEmpty(placa))
                return false;

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

        private bool ValidarCPF(string cpf)
        {
            if (string.IsNullOrEmpty(cpf))
                return false;

            cpf = cpf.Replace(".", "").Replace("-", "").Replace(" ", "");

            if (cpf.Length != 11)
                return false;

            if (cpf.All(c => c == cpf[0]))
                return false;

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
