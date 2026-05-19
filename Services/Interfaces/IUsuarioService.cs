using TreinoSportAPI.Models;

namespace TreinoSportAPI.Services.Interfaces {
    /// <summary>
    /// Serviço de operações relacionadas a usuários e busca de CTs.
    /// </summary>
    public interface IUsuarioService {
        /// <summary>
        /// Busca CTs por localização. Usa coordenadas se fornecidas, ou CEP como fallback via ViaCEP.
        /// </summary>
        Task<List<CTResult>> BuscarCTs(double? lat, double? lng, string cep, int raio);
    }
}
