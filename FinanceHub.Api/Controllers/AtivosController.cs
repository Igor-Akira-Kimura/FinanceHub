using FinanceHub.Api.Interfaces.Services;
using FinanceHub.Api.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FinanceHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtivosController : ControllerBase
    {
        private readonly IAtivoService _ativoService;

        public AtivosController(IAtivoService ativoService)
        {
            _ativoService = ativoService;
        }

        [HttpPost]
        public async Task<IActionResult> Criar(CriarAtivoRequest request)
        {
            var ativo = await _ativoService.CriarAsync(request);

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = ativo.Id },
                ativo);
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodos()
        {
            var ativos = await _ativoService.BuscarTodosAsync();

            return Ok(ativos);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> BuscarPorId(Guid id)
        {
            var ativo = await _ativoService.BuscarPorIdAsync(id);

            return Ok(ativo);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, AtualizarAtivoRequest request)
        {
            await _ativoService.AtualizarAsync(id, request);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Desativar(Guid id)
        {
            await _ativoService.DesativarAsync(id);
            return NoContent();
        }
    }
}
