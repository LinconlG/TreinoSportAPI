using TreinoSportAPI.Mappers;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Services {
    public class ContaService {

        private readonly ContaMapper _usuarioMapper;

        public ContaService(ContaMapper usuarioMapper) {
            _usuarioMapper = usuarioMapper;
        }

        public async Task CadastrarUsuario(Conta usuario) {
            await _usuarioMapper.CadastrarUsuario(usuario);
        }

        public async Task<bool> ChecarEmail(string email) {
            return await _usuarioMapper.ChecarEmail(email);
        }
    }
}
