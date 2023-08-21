using TreinoSportAPI.Mappers;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Services {
    public class LoginService {
        private readonly LoginMapper _loginMapper;
        public LoginService(LoginMapper loginMapper) {
            _loginMapper = loginMapper;
        }

        public async Task<Conta> Login(string email, string senha) {
            return await _loginMapper.CheckLogin(email, senha);
        }
    }
}
