using TreinoSportAPI.Mappers;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Services {
    public class LoginService : ILoginService {
        private readonly ILoginMapper _loginMapper;
        public LoginService(ILoginMapper loginMapper) {
            _loginMapper = loginMapper;
        }

        /// <summary>
        /// Realiza o login verificando email e senha com BCrypt. Retorna a conta correspondente ou lança exceção se inválido.
        /// </summary>
        public async Task<Conta> Login(string email, string senha) {
            var conta = await _loginMapper.CheckLogin(email);
            if (conta == null || !BCrypt.Net.BCrypt.Verify(senha, conta.Senha)) {
                throw new APIException("Credenciais inválidas.", true);
            }
            return conta;
        }
    }
}

