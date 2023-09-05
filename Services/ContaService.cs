using TreinoSportAPI.Mappers;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Services {
    public class ContaService {

        private readonly ContaMapper _usuarioMapper;
        private readonly TreinoService _treinoService;

        public ContaService(ContaMapper usuarioMapper, TreinoService treinoService) {
            _usuarioMapper = usuarioMapper;
            _treinoService = treinoService;
        }

        public async Task<bool> CadastrarUsuario(Conta usuario) {
            var emailExiste = await _usuarioMapper.ChecarEmail(usuario.Email);
            if (emailExiste) {
                return true;
            }

            var codigoConta = await _usuarioMapper.CadastrarUsuario(usuario);
            if (usuario.IsCentroTreinamento) {
                await _treinoService.InserirHorarios(codigoConta, new List<DiaDaSemana>());
            }
            return false;
        }

        public async Task<bool> ChecarEmail(string email) {
            return await _usuarioMapper.ChecarEmail(email);
        }
    }
}
