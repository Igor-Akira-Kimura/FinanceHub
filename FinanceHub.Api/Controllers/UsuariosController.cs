using FinanceHub.Domain.Entities;
using FinanceHub.Application.Interfaces.Services;
using FinanceHub.Application.Requests;
using FinanceHub.Application.Responses;
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

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Atualizar(Guid id, AtualizarUsuarioRequest request)
        {
            await _usuarioService.AtualizarAsync(id, request);

            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Desativar(Guid id)
        {
            await _usuarioService.DesativarAsync(id);
            return NoContent();
        }
    }
}