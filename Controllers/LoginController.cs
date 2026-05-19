using Microsoft.AspNetCore.Mvc;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;
using TreinoSportAPI.Services.Interfaces;

namespace TreinoSportAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase {

        private readonly ILoginService _loginService;
        private readonly IAuthService _authService;

        public LoginController(ILoginService loginService, IAuthService authService) {
            _loginService = loginService;
            _authService = authService;
        }

        /// <summary>Autentica o usuário e retorna um token JWT.</summary>
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Conta user) {

            var authenticatedUser = await _authService.Authenticate(user);

            if (authenticatedUser == null)
                return Unauthorized();

            var token = _authService.GenerateToken(authenticatedUser);

            return Ok(new { token });

        }
    }
}
