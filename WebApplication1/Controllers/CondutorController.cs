using Microsoft.AspNetCore.Mvc;
using WebApplication1.Entities;
using WebApplication1.Interfaces;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CondutorController : ControllerBase
    {
        private readonly ICondutorRepository _condutorRepository;
        private readonly ILogger<CondutorController> _logger;

        public CondutorController(ICondutorRepository condutorRepository, ILogger<CondutorController> logger)
        {
            _condutorRepository = condutorRepository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Condutor>>> GetCondutores()
        {
            try
            {
                var condutores = await _condutorRepository.GetAllAsync();
                return Ok(condutores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter condutores");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Condutor>> GetCondutor(int id)
        {
            try
            {
                var condutor = await _condutorRepository.GetByIdAsync(id);

                if (condutor == null)
                {
                    return NotFound("Condutor não encontrado");
                }

                return Ok(condutor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter condutor {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("cpf/{cpf}")]
        public async Task<ActionResult<Condutor>> GetCondutorByCPF(string cpf)
        {
            try
            {
                var condutor = await _condutorRepository.GetByCPFAsync(cpf);

                if (condutor == null)
                {
                    return NotFound("Condutor não encontrado");
                }

                return Ok(condutor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter condutor pelo CPF {CPF}", cpf);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("cnh/{cnh}")]
        public async Task<ActionResult<Condutor>> GetCondutorByCNH(string cnh)
        {
            try
            {
                var condutor = await _condutorRepository.GetByCNHAsync(cnh);

                if (condutor == null)
                {
                    return NotFound("Condutor não encontrado");
                }

                return Ok(condutor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter condutor pela CNH {CNH}", cnh);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Condutor>> CreateCondutor(Condutor condutor)
        {
            try
            {
                if (condutor == null)
                {
                    return BadRequest("Dados de condutor inválidos");
                }

                await _condutorRepository.AddAsync(condutor);
                await _condutorRepository.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCondutor), new { id = condutor.Id }, condutor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar condutor");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCondutor(int id, Condutor condutor)
        {
            try
            {
                if (id != condutor.Id)
                {
                    return BadRequest("ID do condutor não corresponde");
                }

                var existingCondutor = await _condutorRepository.GetByIdAsync(id);
                if (existingCondutor == null)
                {
                    return NotFound("Condutor não encontrado");
                }

                await _condutorRepository.UpdateAsync(condutor);
                await _condutorRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar condutor {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCondutor(int id)
        {
            try
            {
                var condutor = await _condutorRepository.GetByIdAsync(id);
                if (condutor == null)
                {
                    return NotFound("Condutor não encontrado");
                }

                await _condutorRepository.DeleteAsync(condutor);
                await _condutorRepository.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir condutor {Id}", id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }
    }
}
