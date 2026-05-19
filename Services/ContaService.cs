using TreinoSportAPI.Mappers;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Utilities;
using BCrypt.Net;

namespace TreinoSportAPI.Services {
    public class ContaService : IContaService {

        private readonly IContaMapper _contaMapper;
        private readonly IEmailService _emailService;

        public ContaService(IContaMapper usuarioMapper, IEmailService emailService) {
            _contaMapper = usuarioMapper;
            _emailService = emailService;
        }

        /// <summary>
        /// Cadastra um novo usuário. Retorna true se o email já existe.
        /// </summary>
        public async Task<bool> CadastrarUsuario(Conta usuario) {
            var emailExiste = await _contaMapper.ChecarEmail(usuario.Email);
            if (emailExiste) {
                return true;
            }

            usuario.Senha = BCrypt.Net.BCrypt.HashPassword(usuario.Senha);
            await _contaMapper.CadastrarUsuario(usuario);
            return false;
        }

        /// <summary>
        /// Verifica se um email já está cadastrado.
        /// </summary>
        public async Task<bool> ChecarEmail(string email) {
            return await _contaMapper.ChecarEmail(email);
        }
        /// <summary>
        /// Busca uma conta por código ou email.
        /// </summary>
        public Task<Conta> BuscarConta(int? codigoConta = null, string? email = null) {
            return _contaMapper.BuscarConta(codigoConta, email);
        }
        /// <summary>
        /// Envia um token de redefinição de senha para o email informado.
        /// </summary>
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
        /// <summary>
        /// Verifica se o token informado é válido para a conta.
        /// </summary>
        public async Task ChecarToken(int codigoConta, string tokenInserido) {
            var tokens = await _contaMapper.BuscarTokens(codigoConta);
            if (!tokens.Contains(tokenInserido.ToUpper())) {
                throw new APIException("O código é inválido", true);
            }
        }
        /// <summary>
        /// Redefine a senha da conta após validar o token.
        /// </summary>
        public async Task RedefinirSenha(int codigoConta, string novaSenha, string tokenInserido) {
            var tokens = await _contaMapper.BuscarTokens(codigoConta);
            if (tokens.Contains(tokenInserido.ToUpper())) {
                var senhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
                await _contaMapper.AlterarSenha(codigoConta, senhaHash);
                await _contaMapper.DeletarToken(codigoConta);
                return;
            }
            throw new APIException("Erro no token", true);
        }
        /// <summary>
        /// Atualiza os dados da conta.
        /// </summary>
        public Task AtualizarConta(Conta conta) {
            return _contaMapper.AtualizarConta(conta);
        }
    }
}
