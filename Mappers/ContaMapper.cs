using Dapper;
using TreinoSportAPI.Mappers.Connection;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Mappers {
    public class ContaMapper : IContaMapper {

        private readonly SqlConnectionFactory _factory;

        public ContaMapper(SqlConnectionFactory factory) {
            _factory = factory;
        }

        /// <summary>
        /// Cadastra um novo usuário no banco de dados e retorna o código gerado.
        /// </summary>
        public async Task<int> CadastrarUsuario(Conta usuario) {
            const string sql = @"INSERT INTO CONTA(COEMAIL, CODESCRICAO, CONOMECONTA, COSENHA, COISCENTRO, Latitude, Longitude, Cep)
                        OUTPUT INSERTED.COCODCONTA
                        VALUES (@Email, @Descricao, @Nome, @Senha, @IsCentro, @Latitude, @Longitude, @Cep)";
            using var conn = _factory.CreateConnection();
            var result = await conn.QuerySingleAsync<int>(sql, new {
                Email = usuario.Email,
                Descricao = usuario.Descricao,
                Nome = usuario.Nome,
                Senha = usuario.Senha,
                IsCentro = usuario.IsCentroTreinamento,
                Latitude = usuario.Latitude,
                Longitude = usuario.Longitude,
                Cep = usuario.Cep
            });
            return result;
        }

        /// <summary>
        /// Atualiza os dados de nome, descrição e email de uma conta.
        /// </summary>
        public async Task AtualizarConta(Conta conta) {
            const string sql = @"UPDATE CONTA
                SET CONOMECONTA = @Nome, CODESCRICAO = @Descricao, COEMAIL = @Email,
                    Latitude = @Latitude, Longitude = @Longitude, Cep = @Cep
                WHERE COCODCONTA = @Codigo";
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new { conta.Nome, conta.Descricao, conta.Email, conta.Codigo, conta.Latitude, conta.Longitude, conta.Cep });
        }

        /// <summary>
        /// Verifica se um email já está cadastrado no banco de dados.
        /// </summary>
        public async Task<bool> ChecarEmail(string email) {
            const string sql = "SELECT COEMAIL FROM CONTA WHERE COEMAIL = @Email";
            using var conn = _factory.CreateConnection();
            var result = await conn.QueryFirstOrDefaultAsync<string>(sql, new { Email = email });
            return result != null;
        }

        /// <summary>
        /// Busca uma conta por código ou email.
        /// </summary>
        public async Task<Conta> BuscarConta(int? codigoConta = null, string? email = null) {
            var conditions = new List<string>();
            if (codigoConta != null) conditions.Add("COCODCONTA = @CodigoConta");
            if (email != null) conditions.Add("COEMAIL = @Email");
            var where = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

            var sql = $@"SELECT COCODCONTA AS Codigo, CONOMECONTA AS Nome, CODESCRICAO AS Descricao,
                               COEMAIL AS Email, COISCENTRO AS IsCentroTreinamento,
                               Latitude, Longitude, Cep
                        FROM CONTA {where}";
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Conta>(sql, new { CodigoConta = codigoConta, Email = email });
        }

        /// <summary>
        /// Insere um token de redefinição de senha para a conta informada.
        /// </summary>
        public async Task InserirToken(int codigoConta, string token) {
            const string sql = "INSERT INTO TOKEN (TKNCODCONTA, TKNTOKEN) VALUES (@CodigoConta, @Token)";
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new { CodigoConta = codigoConta, Token = token });
        }

        /// <summary>
        /// Retorna todos os tokens de redefinição de senha associados à conta.
        /// </summary>
        public async Task<List<string>> BuscarTokens(int codigoConta) {
            const string sql = "SELECT TKNTOKEN FROM TOKEN WHERE TKNCODCONTA = @CodigoConta";
            using var conn = _factory.CreateConnection();
            var result = await conn.QueryAsync<string>(sql, new { CodigoConta = codigoConta });
            return result.ToList();
        }

        /// <summary>
        /// Altera a senha de uma conta no banco de dados.
        /// </summary>
        public async Task AlterarSenha(int codigoConta, string novaSenha) {
            const string sql = "UPDATE CONTA SET COSENHA = @NovaSenha WHERE COCODCONTA = @CodigoConta";
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new { NovaSenha = novaSenha, CodigoConta = codigoConta });
        }

        /// <summary>
        /// Remove todos os tokens de redefinição de senha da conta.
        /// </summary>
        public async Task DeletarToken(int codigoConta) {
            const string sql = "DELETE FROM TOKEN WHERE TKNCODCONTA = @CodigoConta";
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new { CodigoConta = codigoConta });
        }

        /// <summary>
        /// Busca CTs próximos às coordenadas informadas usando a fórmula de Haversine.
        /// </summary>
        public async Task<List<CTResult>> BuscarCTsPorLocalizacao(double lat, double lng, int raio) {
            const string sql = @"
                SELECT
                    C.COCODCONTA AS Codigo,
                    C.CONOMECONTA AS Nome,
                    C.CODESCRICAO AS Descricao,
                    dbo.fn_DistanciaKm(@Lat, @Lng, C.Latitude, C.Longitude) AS DistanciaKm,
                    STRING_AGG(T.TRMODALIDADE, ',') AS Modalidades
                FROM CONTA C
                LEFT JOIN TREINO T ON T.TRCODCRIADOR = C.COCODCONTA
                WHERE C.COISCENTRO = 1
                  AND C.Latitude IS NOT NULL
                  AND C.Longitude IS NOT NULL
                  AND dbo.fn_DistanciaKm(@Lat, @Lng, C.Latitude, C.Longitude) <= @Raio
                GROUP BY C.COCODCONTA, C.CONOMECONTA, C.CODESCRICAO, C.Latitude, C.Longitude
                ORDER BY DistanciaKm ASC";
            using var conn = _factory.CreateConnection();
            var rawResults = await conn.QueryAsync<dynamic>(sql, new { Lat = lat, Lng = lng, Raio = raio });
            return rawResults.Select(r => new CTResult {
                Codigo = (int)r.Codigo,
                Nome = (string)r.Nome,
                Descricao = (string)(r.Descricao ?? ""),
                DistanciaKm = (double)r.DistanciaKm,
                Modalidades = string.IsNullOrWhiteSpace((string?)r.Modalidades)
                    ? new List<string>()
                    : ((string)r.Modalidades).Split(',').Select(m => m.Trim()).Where(m => !string.IsNullOrEmpty(m)).ToList()
            }).ToList();
        }
    }
}
