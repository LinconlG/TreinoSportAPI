using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;

namespace TreinoSportAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase {

        private readonly ContaService _usuarioService;

        public UsuarioController(ContaService usuarioService) {
            _usuarioService = usuarioService;
        }

        [HttpPut("cadastrar")]
        public async Task<ActionResult<bool>> PutCadastrarUsuario([FromBody] Conta usuario) {

            try {
                var emailExiste = await _usuarioService.CadastrarUsuario(usuario);
                return Ok(emailExiste);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        [HttpGet("email")]
        public async Task<ActionResult<bool>> GetChecarEmail([FromQuery(Name = "email")][Required] string email) {

            try {
                var resultado = await _usuarioService.ChecarEmail(email);
                return Ok(resultado);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }

        }
    }
}
