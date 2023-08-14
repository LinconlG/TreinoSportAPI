using System.Data;
using TreinoSportAPI.Mappers.Connection;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Mappers {
    public class TreinoMapper : BaseMapper {

        public async Task<List<Treino>> GetTreinosAluno(int codigoUsuario) {

            var listaTreinos = new List<Treino>();

            string sql = @"
                        SELECT
                        TRNOMETREINO,
                        TRDESCRICAOTREINO,    
                        TRDATATREINOS
                        FROM TREINO
                        INNER JOIN TREINOUSUARIO TU ON TU.TUCODUSUARIO = @obj1 AND TU.TUTIPOUSUARIO = 0
                        WHERE
                        TRCODTREINO = TU.TUCODTREINO    
            ";

            var parametros = Parametros.Parametrizar(new List<object> { codigoUsuario });

            var dr = Query(sql, parametros);

            while (await dr.ReadAsync()) {
                var treino = new Treino();
                treino.Nome = dr.GetString("TRNOMETREINO");
                treino.Descricao = dr.GetString("TRDESCRICAOTREINO");
                listaTreinos.Add(treino);
            }
            return listaTreinos;
        }
    }
}
