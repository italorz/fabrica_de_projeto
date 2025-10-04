using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoMultaController : ControllerBase
    {
        private readonly ITipoMultaRepository _tipoMultaRepository;
        private readonly ILogger<TipoMultaController> _logger;

        public TipoMultaController(ITipoMultaRepository tipoMultaRepository, ILogger<TipoMultaController> logger)
        {
            _tipoMultaRepository = tipoMultaRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipoMulta>>> GetTiposMulta()
        {
            try
            {
                var tiposMulta = await _tipoMultaRepository.GetAllAsync();
                return Ok(tiposMulta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tipos de multa");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TipoMulta>> GetTipoMulta(int id)
        {
            try
            {
                var tipoMulta = await _tipoMultaRepository.GetByIdAsync(id);

                if (tipoMulta == null)
                {
                    return NotFound("Tipo de multa não encontrado");
                }

                return Ok(tipoMulta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tipo de multa {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("codigo/{codigo}")]
        public async Task<ActionResult<TipoMulta>> GetTipoMultaByCodigo(string codigo)
        {
            try
            {
                var tipoMulta = await _tipoMultaRepository.GetByCodigoAsync(codigo);

                if (tipoMulta == null)
                {
                    return NotFound("Tipo de multa não encontrado");
                }

                return Ok(tipoMulta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tipo de multa pelo código {Codigo}", codigo);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("gravidade/{gravidade}")]
        public async Task<ActionResult<IEnumerable<TipoMulta>>> GetTipoMultaByGravidade(string gravidade)
        {
            try
            {
                var tiposMulta = await _tipoMultaRepository.GetByGravidadeAsync(gravidade);
                return Ok(tiposMulta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tipos de multa pela gravidade {Gravidade}", gravidade);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TipoMulta>> CreateTipoMulta(TipoMulta tipoMulta)
        {
            try
            {
                if (tipoMulta == null)
                {
                    return BadRequest("Dados de tipo de multa inválidos");
                }

                await _tipoMultaRepository.AddAsync(tipoMulta);
                await _tipoMultaRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTipoMulta), new { id = tipoMulta.Id }, tipoMulta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar tipo de multa");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTipoMulta(int id, TipoMulta tipoMulta)
        {
            try
            {
                if (id != tipoMulta.Id)
                {
                    return BadRequest("ID do tipo de multa não corresponde");
                }

                var existingTipoMulta = await _tipoMultaRepository.GetByIdAsync(id);
                if (existingTipoMulta == null)
                {
                    return NotFound("Tipo de multa não encontrado");
                }

                await _tipoMultaRepository.UpdateAsync(tipoMulta);
                await _tipoMultaRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tipo de multa {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTipoMulta(int id)
        {
            try
            {
                var tipoMulta = await _tipoMultaRepository.GetByIdAsync(id);
                if (tipoMulta == null)
                {
                    return NotFound("Tipo de multa não encontrado");
                }

                await _tipoMultaRepository.DeleteAsync(tipoMulta);
                await _tipoMultaRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir tipo de multa {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
