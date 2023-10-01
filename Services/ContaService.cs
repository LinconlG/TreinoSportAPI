using TreinoSportAPI.Mappers;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Services {
    public class ContaService {

        private readonly ContaMapper _contaMapper;
        private readonly IEmailService _emailService;

        public ContaService(ContaMapper usuarioMapper, IEmailService emailService) {
            _contaMapper = usuarioMapper;
            _emailService = emailService;
        }

        public async Task<bool> CadastrarUsuario(Conta usuario) {
            var emailExiste = await _contaMapper.ChecarEmail(usuario.Email);
            if (emailExiste) {
                return true;
            }

            await _contaMapper.CadastrarUsuario(usuario);
            return false;
        }

        public async Task<bool> ChecarEmail(string email) {
            return await _contaMapper.ChecarEmail(email);
        }
        public Task<Conta> BuscarConta(int? codigoConta = null, string? email = null) {
            return _contaMapper.BuscarConta(codigoConta, email);
        }
        public async Task<int> EnviarTokenSenha(string email) {
            var emailExiste = await _contaMapper.ChecarEmail(email);
            if (!emailExiste) {
                throw new APIException("O email informado não existe.", true);
            }
            var conta = await _contaMapper.BuscarConta(email: email);
            var token = UtilEnvironment.GerarToken();
            await _contaMapper.InserirToken(conta.Codigo, token);
            await _emailService.SendPasswordCode(email, token);
            return conta.Codigo;
        }
        public async Task ChecarToken(int codigoConta, string tokenInserido) {
            var tokens = await _contaMapper.BuscarTokens(codigoConta);
            if (!tokens.Contains(tokenInserido.ToUpper())) {
                throw new APIException("O código é inválido", true);
            }
        }
        public async Task RedefinirSenha(int codigoConta, string novaSenha, string tokenInserido) {
            var tokens = await _contaMapper.BuscarTokens(codigoConta);
            if (tokens.Contains(tokenInserido.ToUpper())) {
                await _contaMapper.AlterarSenha(codigoConta, novaSenha);
                await _contaMapper.DeletarToken(codigoConta);
                return;
            }
            throw new APIException("Erro no token", true);
        }
    }
}
