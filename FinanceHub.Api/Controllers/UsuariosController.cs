using FinanceHub.Api.Domain.Entities;
using FinanceHub.Api.Requests;
using Microsoft.AspNetCore.Mvc;

namespace FinanceHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        [HttpGet]
        public IActionResult Teste()
        {
            return Ok("API funcionando!");
        }

        [HttpPost]
        public IActionResult Cadastrar(CriarUsuarioRequest request)
        {
            var usuario = new Usuario(
                request.Nome,
                request.Email,
                request.Senha // depois vamos gerar o Hash
            );

            return Created($"/api/usuarios/{usuario.Id}", usuario);
        }
    }
}