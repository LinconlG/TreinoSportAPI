using Dapper;
using TreinoSportAPI.Mappers.Connection;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Mappers {
    public class LoginMapper : ILoginMapper {

        private readonly SqlConnectionFactory _factory;

        public LoginMapper(SqlConnectionFactory factory) {
            _factory = factory;
        }

        /// <summary>
        /// Busca a conta pelo email para verificação de credenciais.
        /// </summary>
        public async Task<Conta> CheckLogin(string email) {
            const string sql = @"SELECT COCODCONTA AS Codigo, CONOMECONTA AS Nome,
                                        CODESCRICAO AS Descricao, COISCENTRO AS IsCentroTreinamento,
                                        COSENHA AS Senha
                                 FROM CONTA
                                 WHERE COEMAIL = @Email";
            using var conn = _factory.CreateConnection();
            return await conn.QueryFirstOrDefaultAsync<Conta>(sql, new { Email = email });
        }
    }
}

