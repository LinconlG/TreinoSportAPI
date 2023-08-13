using TreinoSportAPI.Mappers.Connection;

namespace TreinoSportAPI.Mappers {
    public class LoginMapper : BaseMapper {

        public async Task CheckLogin(string email, string senha) {
            string sql = @"
                    SELECT
                        USCODUSUARIO
                    FROM USUARIO
                    WHERE
                        USEMAIL = @obj0
                    AND USSENHA = @obj1
            ";

            var parametros = Parametros.Parametrizar(new List<object> { email, senha });

            var dr = Query(sql, parametros);

            if (await dr.ReadAsync()) {
                return;
            }
            throw new KeyNotFoundException();
        }
    }
}
