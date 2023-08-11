using TreinoSportAPI.Mappers;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Services {
    public class UsuarioService {

        private readonly UsuarioMapper _usuarioMapper;

        public UsuarioService(UsuarioMapper usuarioMapper) {
            _usuarioMapper = usuarioMapper;
        }

        public async Task CadastrarUsuario(Usuario usuario) {
            await _usuarioMapper.CadastrarUsuario(usuario);
        }

        public async Task<bool> ChecarEmail(string email) {
            return await _usuarioMapper.ChecarEmail(email);
        }
    }
}
