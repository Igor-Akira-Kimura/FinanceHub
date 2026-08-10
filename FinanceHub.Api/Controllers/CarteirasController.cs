using FinanceHub.Api.Application.Requests.Carteiras;
using FinanceHub.Api.Interfaces.Services;
using FinanceHub.Api.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CarteirasController : ControllerBase
    {
        private readonly ICarteiraService _service;

        public CarteirasController(ICarteiraService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Criar(CriarCarteiraRequest request)
        {
            var id = await _service.CriarAsync(request);

            return CreatedAtAction(nameof(BuscarPorId), new { id }, id);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> BuscarPorId(Guid id)
        {
            var carteira = await _service.BuscarPorIdAsync(id);

            return Ok(carteira);
        }

        [HttpGet("usuario/{usuarioId:guid}")]
        public async Task<IActionResult> BuscarTodas(Guid usuarioId)
        {
            var carteiras =
                await _service.BuscarTodasAsync(usuarioId);

            return Ok(carteiras);
        }

        [HttpGet("minhas")]
        public async Task<IActionResult> BuscarMinhas()
        {
            var response = await _service.BuscarMinhasAsync();

            return Ok(response);
        }

        [HttpPost("comprar")]
        public async Task<IActionResult> Comprar(ComprarAtivoRequest request)
        {
            await _service.ComprarAtivoAsync(request);

            return NoContent();
        }

        [HttpPost("vender")]
        public async Task<IActionResult> Vender(VenderAtivoRequest request)
        {
            await _service.VenderAtivoAsync(request);

            return NoContent();
        }
    }

}
