using TreinoSportAPI.Models;

namespace TreinoSportAPI.Services.Interfaces {
    public interface IContaService {
        Task<bool> CadastrarUsuario(Conta usuario);
        Task<bool> ChecarEmail(string email);
        Task<Conta> BuscarConta(int? codigoConta = null, string? email = null);
        Task<int> EnviarTokenSenha(string email);
        Task ChecarToken(int codigoConta, string tokenInserido);
        Task RedefinirSenha(int codigoConta, string novaSenha, string tokenInserido);
        Task AtualizarConta(Conta conta);
    }
}
