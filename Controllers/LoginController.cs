using Microsoft.AspNetCore.Mvc;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase {

        private readonly LoginService _loginService;
        private readonly AuthService _authService;

        public LoginController(LoginService loginService) {
            _loginService = loginService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Conta user) {

            try {
                var authenticatedUser = await _authService.Authenticate(user);

                if (authenticatedUser == null)
                    return Unauthorized();

                var token = _authService.GenerateToken(authenticatedUser);

                return Ok(new { token });
            }
            catch (Exception e) {
                return UtilEnvironment.InternalServerError(this, e.Message, UtilEnvironment.IsPublicMessageCheck(e));
            }

        }
    }
}
