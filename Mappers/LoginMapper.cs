using System.Data;
using TreinoSportAPI.Mappers.Connection;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Mappers {
    public class LoginMapper : BaseMapper {

        public async Task<Conta> CheckLogin(string email, string senha) {
            string sql = @"
                    SELECT
                        COCODCONTA,
                        CONOMECONTA,
                        CODESCRICAO,
                        COISCENTRO
                    FROM CONTA
                    WHERE
                        COEMAIL = @obj0
                    AND COSENHA = @obj1
            ";

            var parametros = Parametros.Parametrizar(email, senha);

            var dr = Query(sql, parametros);

            if (await dr.ReadAsync()) {
                var conta = new Conta();
                conta.Codigo = dr.GetInt32("COCODCONTA");
                conta.Descricao = dr.IsDBNull("CODESCRICAO") ? null : dr.GetString("CODESCRICAO");
                conta.Nome = dr.GetString("CONOMECONTA");
                conta.IsCentroTreinamento = dr.GetBoolean("COISCENTRO");
                return conta;
            }
            throw new KeyNotFoundException();
        }
    }
}
