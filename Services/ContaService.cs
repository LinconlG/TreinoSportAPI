using TreinoSportAPI.Mappers;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Services {
    public class ContaService {

        private readonly ContaMapper _usuarioMapper;
        private readonly TreinoService _treinoService;
        private readonly IEmailService _emailService;

        public ContaService(ContaMapper usuarioMapper, TreinoService treinoService, IEmailService emailService) {
            _usuarioMapper = usuarioMapper;
            _treinoService = treinoService;
            _emailService = emailService;
        }

        public async Task<bool> CadastrarUsuario(Conta usuario) {
            var emailExiste = await _usuarioMapper.ChecarEmail(usuario.Email);
            if (emailExiste) {
                return true;
            }

            await _usuarioMapper.CadastrarUsuario(usuario);
            return false;
        }

        public async Task<bool> ChecarEmail(string email) {
            return await _usuarioMapper.ChecarEmail(email);
        }
        public Task<Conta> BuscarConta(int? codigoConta = null, string? email = null) {
            return _usuarioMapper.BuscarConta(codigoConta, email);
        }
        public async Task EnviarEmailSenha(string email) {
            var emailExiste = await _usuarioMapper.ChecarEmail(email);
            if (!emailExiste) {
                throw new APIException("O email informado não existe.", true);
            }
            var conta = await _usuarioMapper.BuscarConta(email: email);
            var token = UtilEnvironment.GerarToken();
            await _usuarioMapper.InserirToken(conta.Codigo, token);
            await _emailService.SendPasswordCode(email, "666");
        }
    }
}
