using TreinoSportAPI.Mappers.Connection;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Mappers {
    public class UsuarioMapper : BaseMapper {

        public async Task CadastrarUsuario(Usuario usuario) {

            string sql = @"INSERT INTO USUARIO(USEMAIL, USNOMEUSUARIO, USSENHA)
                        VALUES (@email, @nome, @senha)
            ";
            var parametros = new List<(string, object)> {
                ("email", usuario.Email),
                ("nome", usuario.Nome),
                ("senha", usuario.Senha)
            };

            await NonQuery(sql, parametros);
        }
    }
}
