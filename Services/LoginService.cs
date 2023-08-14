using TreinoSportAPI.Mappers;

namespace TreinoSportAPI.Services {
    public class LoginService {
        private readonly LoginMapper _loginMapper;
        public LoginService(LoginMapper loginMapper) {
            _loginMapper = loginMapper;
        }

        public async Task<int> Login(string email, string senha) {
            return await _loginMapper.CheckLogin(email, senha);
        }
    }
}
