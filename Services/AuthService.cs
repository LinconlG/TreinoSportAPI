using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Services {
    public class AuthService {
        private readonly IConfiguration _configuration;
        private readonly LoginService _loginService;

        public AuthService(IConfiguration configuration, LoginService loginService) {
            _configuration = configuration;
            _loginService = loginService;
        }


        public async Task<Conta?> Authenticate(Conta user) {
            var conta = await _loginService.Login(user.Email, user.Senha);
            if (conta != null) {
                return conta;
            }
            return null;
        }

        public string GenerateToken(Conta user) {

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);

            //tem as infos para gerar o token
            var tokenDescriptor = new SecurityTokenDescriptor {
                // Define as claims (informações) do usuário
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Email)
                }),
                Issuer = _configuration["Jwt:Issuer"]!,
                Audience = _configuration["Jwt:Audience"]!,

                // Define o tempo de expiração do token
                Expires = DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]!)),
                NotBefore = DateTime.Now, // Válido imediatamente
                IssuedAt = DateTime.Now, // Emitido agora
                // Define as credenciais de assinatura (como sera assinado)
                SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
            };

            // Cria um manipulador de tokens JWT
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

        }
    }
}
