using System.Data;
using TreinoSportAPI.Mappers.Connection;
using TreinoSportAPI.Models;
using TreinoSportAPI.Models.Enums;

namespace TreinoSportAPI.Mappers {
    public class TreinoMapper : BaseMapper {

        public async Task<List<Treino>> GetTreinosComoAluno(int codigoUsuario) {

            var listaTreinos = new List<Treino>();

            string sql = @"
                        SELECT
                            TRCODTREINO,
                            TRNOMETREINO,
                            CO.CONOMECONTA,
                            TRDESCRICAOTREINO,    
                            TRDATACRIACAO,
                            TRDATAVENCIMENTO,
                            TRMODALIDADE
                        FROM TREINO
                        INNER JOIN CONTA CO ON CO.COCODCONTA = TRCODCRIADOR
                        INNER JOIN TREINOALUNO TA ON TA.TACODALUNO = @obj0 AND TA.TACODTREINO = TRCODTREINO
            ";

            var parametros = Parametros.Parametrizar(new List<object> { codigoUsuario });

            var dr = Query(sql, parametros);

            while (await dr.ReadAsync()) {
                var treino = new Treino();
                treino.Nome = dr.GetString("TRNOMETREINO");
                treino.Descricao = dr.GetString("TRDESCRICAOTREINO");
                treino.Criador = new();
                treino.Criador.Nome = dr.GetString("CONOMECONTA");
                listaTreinos.Add(treino);
            }
            return listaTreinos;
        }
        public async Task<List<Treino>> GetTreinosComoCT(int codigoCT) {

            var listaTreinos = new List<Treino>();

            string sql = $@"
                        SELECT
	                        TR.TRCODTREINO,
	                        TR.TRNOMETREINO
                        FROM TREINO TR
                        WHERE
                            TR.TRCODCRIADOR = @obj0
            ";
            var parametros = Parametros.Parametrizar(new List<object> { codigoCT });

            var dr = Query(sql, parametros);

            while (await dr.ReadAsync()) {
                var treino = new Treino();
                treino.Codigo = dr.GetInt32("TRCODTREINO");
                treino.Nome = dr.GetString("TRNOMETREINO");
                listaTreinos.Add(treino);
            }
            return listaTreinos;
        }
        public async Task<Treino> BuscarTreinoPorCodigo(int codigoTreino) {

            string sql = $@"
                        SELECT
	                        TRCODTREINO,
	                        TRDESCRICAOTREINO,
                            TRDATAVENCIMENTO,
                            TRMODALIDADE
                        FROM TREINO
                        WHERE
                            TRCODTREINO = @obj0
            ";
            var parametros = Parametros.Parametrizar(new List<object> { codigoTreino });

            var dr = Query(sql, parametros);

            if (await dr.ReadAsync()) {
                var treino = new Treino();
                treino.Codigo = dr.GetInt32("TRCODTREINO");
                treino.Descricao = dr.GetString("TRDESCRICAOTREINO");
                treino.DataVencimento = dr.GetDateTime("TRDATAVENCIMENTO");
                treino.Modalidade = (ModalidadeTreino) dr.GetByte("TRMODALIDADE");
                return treino;
            }
            return new Treino();
        }
        public async Task<int> InserirTreino(Treino treino) {
            string sql = @"
                INSERT INTO TREINO 
                     (
                         TRNOMETREINO,
                         TRDESCRICAOTREINO,
                         TRDATACRIACAO,
                         TRDATAVENCIMENTO,
                         TRCODCRIADOR,
                         TRMODALIDADE
                     )
                    OUTPUT INSERTED.TRCODTREINO
                VALUES
                    (
                        @obj0,
                        @obj1,
                        @obj2,
                        @obj3,
                        @obj4,
                        @obj5
                    )
            ";

            var parametros = Parametros.Parametrizar(new List<object> { treino.Nome, treino.Descricao, DateTime.Now, treino.DataVencimento, treino.Criador.Codigo, treino.Modalidade });

            var dr =Query(sql, parametros);

            if (await dr.ReadAsync()) {
                return dr.GetInt32("TRCODTREINO");
            }
            return 0;
        }
    }
}
