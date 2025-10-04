using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MultaController : ControllerBase
    {
        private readonly IMultaRepository _multaRepository;
        private readonly ILogger<MultaController> _logger;

        public MultaController(IMultaRepository multaRepository, ILogger<MultaController> logger)
        {
            _multaRepository = multaRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Multa>>> GetMultas()
        {
            try
            {
                var multas = await _multaRepository.GetAllAsync();
                return Ok(multas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter multas");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Multa>> GetMulta(int id)
        {
            try
            {
                var multa = await _multaRepository.GetByIdAsync(id);

                if (multa == null)
                {
                    return NotFound("Multa não encontrada");
                }

                return Ok(multa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter multa {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("veiculo/{veiculoId}")]
        public async Task<ActionResult<IEnumerable<Multa>>> GetMultasByVeiculo(int veiculoId)
        {
            try
            {
                var multas = await _multaRepository.GetByVeiculoIdAsync(veiculoId);
                return Ok(multas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter multas do veículo {VeiculoId}", veiculoId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("condutor/{condutorId}")]
        public async Task<ActionResult<IEnumerable<Multa>>> GetMultasByCondutor(int condutorId)
        {
            try
            {
                var multas = await _multaRepository.GetByCondutorIdAsync(condutorId);
                return Ok(multas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter multas do condutor {CondutorId}", condutorId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Multa>>> GetMultasByUsuario(int usuarioId)
        {
            try
            {
                var multas = await _multaRepository.GetByUsuarioIdAsync(usuarioId);
                return Ok(multas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter multas do usuário {UsuarioId}", usuarioId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("tipomulta/{tipoMultaId}")]
        public async Task<ActionResult<IEnumerable<Multa>>> GetMultasByTipoMulta(int tipoMultaId)
        {
            try
            {
                var multas = await _multaRepository.GetByTipoMultaIdAsync(tipoMultaId);
                return Ok(multas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter multas do tipo {TipoMultaId}", tipoMultaId);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("periodo")]
        public async Task<ActionResult<IEnumerable<Multa>>> GetMultasByPeriodo(
            [FromQuery] DateTime dataInicio, 
            [FromQuery] DateTime dataFim)
        {
            try
            {
                var multas = await _multaRepository.GetByDataAsync(dataInicio, dataFim);
                return Ok(multas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter multas por período");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Multa>> CreateMulta(Multa multa)
        {
            try
            {
                if (multa == null)
                {
                    return BadRequest("Dados de multa inválidos");
                }

                await _multaRepository.AddAsync(multa);
                await _multaRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetMulta), new { id = multa.Id }, multa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar multa");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMulta(int id, Multa multa)
        {
            try
            {
                if (id != multa.Id)
                {
                    return BadRequest("ID da multa não corresponde");
                }

                var existingMulta = await _multaRepository.GetByIdAsync(id);
                if (existingMulta == null)
                {
                    return NotFound("Multa não encontrada");
                }

                await _multaRepository.UpdateAsync(multa);
                await _multaRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar multa {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMulta(int id)
        {
            try
            {
                var multa = await _multaRepository.GetByIdAsync(id);
                if (multa == null)
                {
                    return NotFound("Multa não encontrada");
                }

                await _multaRepository.DeleteAsync(multa);
                await _multaRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir multa {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
