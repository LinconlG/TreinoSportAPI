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
    }
}
