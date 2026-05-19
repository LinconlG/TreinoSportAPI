using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;

namespace TreinoSportAPI.Mappers.Interfaces {
    public interface IContaMapper {
        Task<int> CadastrarUsuario(Conta usuario);
        Task AtualizarConta(Conta conta);
        Task<bool> ChecarEmail(string email);
        Task<Conta> BuscarConta(int? codigoConta = null, string? email = null);
        Task InserirToken(int codigoConta, string token);
        Task<List<string>> BuscarTokens(int codigoConta);
        Task AlterarSenha(int codigoConta, string novaSenha);
        Task DeletarToken(int codigoConta);
        /// <summary>
        /// Busca CTs próximos às coordenadas informadas usando a fórmula de Haversine.
        /// </summary>
        Task<List<CTResult>> BuscarCTsPorLocalizacao(double lat, double lng, int raio);
    }
}
