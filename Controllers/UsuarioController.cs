using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.ComponentModel.DataAnnotations;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase {

        private readonly IContaService _usuarioService;
        private readonly IUsuarioService _ctService;

        public UsuarioController(IContaService usuarioService, IUsuarioService ctService) {
            _usuarioService = usuarioService;
            _ctService = ctService;
        }

        /// <summary>Cadastra um novo usuário na plataforma.</summary>
        [HttpPut("cadastrar")]
        public async Task<ActionResult<bool>> PutCadastrarUsuario([FromBody] Conta usuario) {

            var emailExiste = await _usuarioService.CadastrarUsuario(usuario);
            if (emailExiste) {
                throw new APIException("Email já existe.", true);
            }
            return Ok();

        }

        /// <summary>Verifica se um email já está cadastrado.</summary>
        [HttpGet("email")]
        public async Task<ActionResult<bool>> GetChecarEmail([FromQuery(Name = "email")][Required] string email) {

            var resultado = await _usuarioService.ChecarEmail(email);
            return Ok(resultado);

        }

        /// <summary>Busca os dados de uma conta pelo código.</summary>
        [HttpGet("conta/codigo")]
        public async Task<ActionResult<Conta>> GetConta([FromQuery(Name = "codigoConta")][Required] int codigoConta) {

            var conta = await _usuarioService.BuscarConta(codigoConta);
            return Ok(conta);

        }

        /// <summary>Envia um token de redefinição de senha para o email informado.</summary>
        [EnableRateLimiting("PasswordReset")]
        [HttpPut("senha/envio")]
        public async Task<ActionResult<int>> PutEnviarTokenSenha([FromQuery(Name = "email")] string email) {

            var codigoConta = await _usuarioService.EnviarTokenSenha(email);
            return Ok(codigoConta);

        }

        /// <summary>Verifica se o token de redefinição de senha é válido.</summary>
        [HttpGet("token")]
        public async Task<ActionResult> GetChecarTokenSenha([FromQuery(Name = "codigoConta")] int codigoConta, [FromQuery(Name = "tokenInserido")] string tokenInserido) {

            await _usuarioService.ChecarToken(codigoConta, tokenInserido);
            return Ok();

        }

        /// <summary>Redefine a senha da conta após validar o token.</summary>
        [HttpPut("senha/redefinir")]
        public async Task<ActionResult> PutRedefinirSenha([FromQuery(Name = "codigoConta")] int codigoConta, [FromQuery(Name = "novaSenha")] string novaSenha, [FromQuery(Name = "tokenInserido")] string tokenInserido) {

            await _usuarioService.RedefinirSenha(codigoConta, novaSenha, tokenInserido);
            return Ok();

        }

        /// <summary>Atualiza os dados da conta autenticada.</summary>
        [Authorize]
        [HttpPatch("atualizar")]
        public async Task<ActionResult> PatchConta([FromBody] Conta conta) {

            var codigoContaToken = int.Parse(User.FindFirst("CodigoConta")!.Value);
            if (conta.Codigo != codigoContaToken) {
                return Forbid();
            }

            await _usuarioService.AtualizarConta(conta);
            return Ok();

        }

        /// <summary>Busca Centros de Treinamento próximos por coordenadas ou CEP.</summary>
        [HttpGet("ct/buscar")]
        public async Task<ActionResult<List<CTResult>>> GetBuscarCTs(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] string cep,
            [FromQuery] int raio = 20) {

            var resultado = await _ctService.BuscarCTs(latitude, longitude, cep, raio);
            return Ok(resultado);

        }
    }
}
