using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Identity.Client;
using System.Data;
using TreinoSportAPI.Mappers.Connection;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Mappers {
    public class ContaMapper : BaseMapper {

        public async Task<int> CadastrarUsuario(Conta usuario) {

            string sql = @"INSERT INTO CONTA(COEMAIL, CODESCRICAO, CONOMECONTA, COSENHA, COISCENTRO)
                        OUTPUT INSERTED.COCODCONTA
                        VALUES (@email, @descricao, @nome, @senha, @isCentro)
            ";
            var parametros = new List<(string, object)> {
                ("email", usuario.Email),
                ("nome", usuario.Nome),
                ("senha", usuario.Senha),
                ("descricao", usuario.Descricao),
                ("isCentro", usuario.IsCentroTreinamento)
            };

            var dr = Query(sql, parametros);

            if (await dr.ReadAsync()) {
                return dr.GetInt32("COCODCONTA");
            }
            throw new Exception("Erro ao cadastrar");
        }

        public async Task<bool> ChecarEmail(string email) {
            string sql = @"
                    SELECT 
                        COEMAIL
                    FROM CONTA
                    WHERE
                        COEMAIL = @email
            ";

            var parametros = new List<(string, object)> {
                ("email", email)
            };

            var dr = Query(sql, parametros);

            if (await dr.ReadAsync()) {
                return true;
            }
            return false;
        }
        public async Task<Conta> BuscarConta(int? codigoConta = null, string? email = null) {
            string sql = @$"
                        SELECT
                            COCODCONTA,
                            CONOMECONTA,
                            CODESCRICAO,
                            COEMAIL,
                            COISCENTRO
                        FROM CONTA
                        WHERE
                        {(codigoConta != null ? "COCODCONTA = @obj0" : "")}
                        {((codigoConta != null && email != null) ? " AND " : "")}
                        {(email != null ? "COEMAIL = @obj1" : "")}
            ";

            var parametros = Parametros.Parametrizar(new List<object> { codigoConta, email });

            var dr = Query(sql, parametros);

            if (await dr.ReadAsync()) {
                var conta = new Conta();
                conta.Codigo = dr.GetInt32("COCODCONTA");
                conta.Nome = dr.GetString("CONOMECONTA");
                conta.Email = dr.GetString("COEMAIL");
                conta.Descricao = dr.IsDBNull("CODESCRICAO") ? null : dr.GetString("CODESCRICAO");
                conta.IsCentroTreinamento = dr.GetBoolean("COISCENTRO");
                return conta;
            }
            return null;
        }
    }
}
