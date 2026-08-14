using FinanceHub.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace FinanceHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BolsasController : ControllerBase
    {
        private readonly IBolsaService _bolsaService;

        public BolsasController(IBolsaService bolsaService)
        {
            _bolsaService = bolsaService;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodas()
        {
            var bolsas = await _bolsaService.BuscarTodasAsync();

            return Ok(bolsas);
        }
    }
}
