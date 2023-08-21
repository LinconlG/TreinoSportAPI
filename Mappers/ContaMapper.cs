using TreinoSportAPI.Mappers.Connection;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Mappers {
    public class ContaMapper : BaseMapper {

        public async Task CadastrarUsuario(Conta usuario) {

            string sql = @"INSERT INTO CONTA(COEMAIL, CODESCRICAO, CONOMECONTA, COSENHA, COISCENTRO)
                        VALUES (@email, @descricao, @nome, @senha, @isCentro)
            ";
            var parametros = new List<(string, object)> {
                ("email", usuario.Email),
                ("nome", usuario.Nome),
                ("senha", usuario.Senha),
                ("descricao", usuario.Senha),
                ("isCentro", usuario.Senha)
            };

            await NonQuery(sql, parametros);
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
    }
}
