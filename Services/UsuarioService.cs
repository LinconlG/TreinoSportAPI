using System.Text.Json;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Services {
    /// <summary>
    /// Serviço de operações relacionadas a usuários e busca de CTs.
    /// </summary>
    public class UsuarioService : IUsuarioService {

        private readonly IContaMapper _contaMapper;
        private readonly HttpClient _httpClient;

        public UsuarioService(IContaMapper contaMapper, HttpClient httpClient) {
            _contaMapper = contaMapper;
            _httpClient = httpClient;
        }

        /// <summary>
        /// Busca CTs por localização. Usa coordenadas se fornecidas, ou CEP como fallback via ViaCEP.
        /// </summary>
        public async Task<List<CTResult>> BuscarCTs(double? lat, double? lng, string cep, int raio) {
            double resolvedLat;
            double resolvedLng;

            if (lat.HasValue && lng.HasValue) {
                resolvedLat = lat.Value;
                resolvedLng = lng.Value;
            }
            else if (!string.IsNullOrWhiteSpace(cep)) {
                var coords = await ResolverCoordsViaCep(cep);
                resolvedLat = coords.lat;
                resolvedLng = coords.lng;
            }
            else {
                throw new APIException("Informe as coordenadas ou um CEP para buscar CTs.", true);
            }

            return await _contaMapper.BuscarCTsPorLocalizacao(resolvedLat, resolvedLng, raio);
        }

        private async Task<(double lat, double lng)> ResolverCoordsViaCep(string cep) {
            var cepLimpo = cep.Replace("-", "").Trim();
            if (cepLimpo.Length != 8) {
                throw new APIException("CEP inválido. O CEP deve ter 8 dígitos.", true);
            }

            HttpResponseMessage response;
            try {
                response = await _httpClient.GetAsync($"https://viacep.com.br/ws/{cepLimpo}/json/");
            }
            catch (Exception) {
                throw new APIException("Não foi possível consultar o serviço ViaCEP. Tente novamente.", true);
            }

            if (!response.IsSuccessStatusCode) {
                throw new APIException("CEP não encontrado.", true);
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("erro", out _)) {
                throw new APIException("CEP não encontrado.", true);
            }

            if (!root.TryGetProperty("localidade", out var localidadeEl) ||
                !root.TryGetProperty("uf", out var ufEl)) {
                throw new APIException("Não foi possível obter a cidade a partir do CEP informado.", true);
            }

            var localidade = localidadeEl.GetString() ?? "";
            var uf = ufEl.GetString() ?? "";

            // Use Nominatim to geocode city + state
            var nominatimUrl = $"https://nominatim.openstreetmap.org/search?city={Uri.EscapeDataString(localidade)}&state={Uri.EscapeDataString(uf)}&country=Brazil&format=json";
            var nominatimRequest = new HttpRequestMessage(HttpMethod.Get, nominatimUrl);
            nominatimRequest.Headers.Add("User-Agent", "TreinoSportAPI/1.0");

            HttpResponseMessage nominatimResponse;
            try {
                nominatimResponse = await _httpClient.SendAsync(nominatimRequest);
            }
            catch (Exception) {
                throw new APIException("Não foi possível consultar o serviço de geocodificação. Tente novamente.", true);
            }

            if (!nominatimResponse.IsSuccessStatusCode) {
                throw new APIException("Erro ao buscar coordenadas para o CEP informado.", true);
            }

            var nominatimJson = await nominatimResponse.Content.ReadAsStringAsync();
            using var nominatimDoc = JsonDocument.Parse(nominatimJson);
            var results = nominatimDoc.RootElement;

            if (results.ValueKind != JsonValueKind.Array || results.GetArrayLength() == 0) {
                throw new APIException("Não foi possível encontrar coordenadas para o CEP informado. Por favor, permita o acesso à sua localização no navegador.", true);
            }

            var first = results[0];
            if (!first.TryGetProperty("lat", out var latEl) || !first.TryGetProperty("lon", out var lonEl)) {
                throw new APIException("Resposta inválida do serviço de geocodificação.", true);
            }

            if (!double.TryParse(latEl.GetString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var resolvedLat) ||
                !double.TryParse(lonEl.GetString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var resolvedLng)) {
                throw new APIException("Coordenadas inválidas retornadas pelo serviço de geocodificação.", true);
            }

            return (resolvedLat, resolvedLng);
        }
    }
}
