using Microsoft.AspNetCore.Mvc;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase {

        private readonly LoginService _loginService;

        public LoginController(LoginService loginService) {
            _loginService = loginService;
        }

        [HttpGet]
        public async Task<ActionResult<Conta>> Login([FromQuery(Name = "email")] string email, [FromQuery(Name = "senha")] string senha) {
            try {
                var codigoUsuario = await _loginService.Login(email, senha);
                return Ok(codigoUsuario);
            }
            catch (Exception e) {
                return UtilEnvironment.InternalServerError(this, e.Message, UtilEnvironment.IsPublicMessageCheck(e));
            }
        }
    }
}
