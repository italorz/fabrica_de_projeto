using WebApplication1.Entities;

namespace WebApplication1.Interfaces
{
    public interface IAIProcessingService
    {
        Task<string> ProcessarImagemMultaAsync(byte[] imagemBytes);
        Task<MultaData> ExtrairDadosMultaAsync(string jsonData);
        Task<bool> ValidarDadosMultaAsync(MultaData dados);
        Task<string> ConverterParaJsonAsync(MultaData dados);
    }

    public class MultaData
    {
        public string? PlacaVeiculo { get; set; }
        public string? NomeCondutor { get; set; }
        public string? CPFCondutor { get; set; }
        public string? CNHCondutor { get; set; }
        public DateTime? DataHora { get; set; }
        public string? Endereco { get; set; }
        public string? Descricao { get; set; }
        public string? CodigoMulta { get; set; }
        public string? MarcaVeiculo { get; set; }
        public string? ModeloVeiculo { get; set; }
        public string? TipoVeiculo { get; set; }
        public string? ProprietarioVeiculo { get; set; }
        public string? CategoriaCNH { get; set; }
        public string? CPF_UF { get; set; }
    }
}
