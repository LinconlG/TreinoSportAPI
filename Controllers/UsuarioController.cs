using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
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

        [HttpGet("conta/codigo")]
        public async Task<ActionResult<Conta>> GetConta([FromQuery(Name = "codigoConta")][Required] int codigoConta) {

            try {
                var conta = await _usuarioService.BuscarConta(codigoConta);
                return Ok(conta);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }

        }

        [HttpPut("senha/envio")]
        public async Task<ActionResult<int>> PutEnviarTokenSenha([FromQuery(Name = "email")] string email) {

            try {
                var codigoConta = await _usuarioService.EnviarTokenSenha(email);
                return Ok(codigoConta);
            }
            catch (Exception e) {
                return UtilEnvironment.InternalServerError(this, e.Message, UtilEnvironment.IsPublicMessageCheck(e));
            }

        }

        [HttpGet("token")]
        public async Task<ActionResult> GetChecarTokenSenha([FromQuery(Name = "codigoConta")] int codigoConta, [FromQuery(Name = "tokenInserido")] string tokenInserido) {

            try {
                await _usuarioService.ChecarToken(codigoConta, tokenInserido);
                return Ok();
            }
            catch (Exception e) {
                return UtilEnvironment.InternalServerError(this, e.Message, UtilEnvironment.IsPublicMessageCheck(e));
            }

        }

        [HttpPut("senha/redefinir")]
        public async Task<ActionResult> PutRedefinirSenha([FromQuery(Name = "codigoConta")] int codigoConta, [FromQuery(Name = "novaSenha")] string novaSenha, [FromQuery(Name = "tokenInserido")] string tokenInserido) {

            try {
                await _usuarioService.RedefinirSenha(codigoConta, novaSenha, tokenInserido);
                return Ok();
            }
            catch (Exception e) {
                return this.InternalServerError(e.Message, e.IsPublicMessageCheck());//okfpsdokfpsdjvfdflkvmfdklvmfdlkmvlkdvmlçlscd
            }

        }
        [HttpPatch("atualizar")]
        public async Task<ActionResult> PatchConta([FromBody] Conta conta) {

            try {
                await _usuarioService.AtualizarConta(conta);
                return Ok();
            }
            catch (Exception e) {
                return this.InternalServerError(e.Message, e.IsPublicMessageCheck());
            }

        }
    }
}
