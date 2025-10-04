using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VeiculoController : ControllerBase
    {
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly ILogger<VeiculoController> _logger;

        public VeiculoController(IVeiculoRepository veiculoRepository, ILogger<VeiculoController> logger)
        {
            _veiculoRepository = veiculoRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculos()
        {
            try
            {
                var veiculos = await _veiculoRepository.GetAllAsync();
                return Ok(veiculos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículos");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Veiculo>> GetVeiculo(int id)
        {
            try
            {
                var veiculo = await _veiculoRepository.GetByIdAsync(id);

                if (veiculo == null)
                {
                    return NotFound("Veículo não encontrado");
                }

                return Ok(veiculo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículo {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("placa/{placa}")]
        public async Task<ActionResult<Veiculo>> GetVeiculoByPlaca(string placa)
        {
            try
            {
                var veiculo = await _veiculoRepository.GetByPlacaAsync(placa);

                if (veiculo == null)
                {
                    return NotFound("Veículo não encontrado");
                }

                return Ok(veiculo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículo pela placa {Placa}", placa);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("proprietario/{proprietario}")]
        public async Task<ActionResult<IEnumerable<Veiculo>>> GetVeiculosByProprietario(string proprietario)
        {
            try
            {
                var veiculos = await _veiculoRepository.GetByProprietarioAsync(proprietario);
                return Ok(veiculos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter veículos pelo proprietário {Proprietario}", proprietario);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Veiculo>> CreateVeiculo(Veiculo veiculo)
        {
            try
            {
                if (veiculo == null)
                {
                    return BadRequest("Dados de veículo inválidos");
                }

                await _veiculoRepository.AddAsync(veiculo);
                await _veiculoRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetVeiculo), new { id = veiculo.Id }, veiculo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar veículo");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVeiculo(int id, Veiculo veiculo)
        {
            try
            {
                if (id != veiculo.Id)
                {
                    return BadRequest("ID do veículo não corresponde");
                }

                var existingVeiculo = await _veiculoRepository.GetByIdAsync(id);
                if (existingVeiculo == null)
                {
                    return NotFound("Veículo não encontrado");
                }

                await _veiculoRepository.UpdateAsync(veiculo);
                await _veiculoRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar veículo {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVeiculo(int id)
        {
            try
            {
                var veiculo = await _veiculoRepository.GetByIdAsync(id);
                if (veiculo == null)
                {
                    return NotFound("Veículo não encontrado");
                }

                await _veiculoRepository.DeleteAsync(veiculo);
                await _veiculoRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir veículo {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
