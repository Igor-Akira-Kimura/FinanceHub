using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Interfaces.Services;
using FinanceHub.Api.Requests;
using FinanceHub.Api.Responses;
using Microsoft.AspNetCore.Mvc;

namespace FinanceHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        
        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> BuscarTodos()
        {
            var response = await _usuarioService.BuscarTodosAsync();

            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CriarUsuarioRequest request)
        {
            var response = await _usuarioService.CadastrarAsync(request);

            return Created($"/api/usuarios/{response.Id}", response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> BuscarPorId(Guid id)
        {
            var response = await _usuarioService.BuscarPorIdAsync(id);

            return Ok(response);
        }
    }
}