using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Requests;
using FinanceHub.Api.Services.Interfaces;
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
        public IActionResult Teste()
        {
            return Ok("API funcionando!");
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(CriarUsuarioRequest request)
        {
            var response = await _usuarioService.CadastrarAsync(request);

            return Created($"/api/usuarios/{response.Id}", response);
        }
    }
}