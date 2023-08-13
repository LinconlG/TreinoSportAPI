using Microsoft.AspNetCore.Mvc;
using TreinoSportAPI.Services;

namespace TreinoSportAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase {

        private readonly LoginService _loginService;

        public LoginController(LoginService loginService) {
            _loginService = loginService;
        }

        [HttpGet]
        public async Task<ActionResult> Login([FromQuery(Name = "email")] string email, [FromQuery(Name = "senha")] string senha) {
            try {
                await _loginService.Login(email, senha);
                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
