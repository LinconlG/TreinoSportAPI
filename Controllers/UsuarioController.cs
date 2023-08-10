using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;

namespace TreinoSportAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase {

        private readonly UsuarioService _usuarioService;

        public UsuarioController(UsuarioService usuarioService) {
            _usuarioService = usuarioService;
        }

        [HttpPut("cadastrar")]
        public async Task<ActionResult> PutCadastrarUsuario([FromBody] Usuario usuario) {

            try {
                await _usuarioService.CadastrarUsuario(usuario);
                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }

        }
    }
}
