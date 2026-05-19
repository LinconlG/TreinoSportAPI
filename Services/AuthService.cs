using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services.Interfaces;

namespace TreinoSportAPI.Services {
    public class AuthService : IAuthService {
        private readonly IConfiguration _configuration;
        private readonly ILoginService _loginService;

        public AuthService(IConfiguration configuration, ILoginService loginService) {
            _configuration = configuration;
            _loginService = loginService;
        }


        /// <summary>
        /// Autentica um usuário verificando email e senha. Retorna a conta autenticada ou null se inválido.
        /// </summary>
        public async Task<Conta?> Authenticate(Conta user) {
            try {
                var conta = await _loginService.Login(user.Email, user.Senha);
                if (conta != null) {
                    conta.Email = user.Email;
                    return conta;
                }
                return null;
            }
            catch {
                return null;
            }
        }

        /// <summary>
        /// Gera um token JWT para a conta autenticada.
        /// </summary>
        public string GenerateToken(Conta user) {

            var key = Encoding.ASCII.GetBytes(
                Environment.GetEnvironmentVariable("JWT_SECRET") 
                ?? _configuration["Jwt:Key"] 
                ?? throw new InvalidOperationException("JWT secret not configured."));

            //tem as infos para gerar o token
            var tokenDescriptor = new SecurityTokenDescriptor {
                // Define as claims (informações) do usuário
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Role, user.IsCentroTreinamento ? "CT" : "Aluno"),
                    new Claim("CodigoConta", user.Codigo.ToString())
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
