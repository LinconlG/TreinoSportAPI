﻿using System.Data;
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

            var parametros = Parametrizar(codigoUsuario);

            var dr = Query(sql, parametros);

            while (await dr.ReadAsync()) {
                var treino = new Treino();
                treino.Codigo = dr.GetInt32("TRCODTREINO");
                treino.Nome = dr.GetString("TRNOMETREINO");
                treino.Descricao = dr.GetString("TRDESCRICAOTREINO");
                treino.Criador = new();
                treino.Criador.Nome = dr.GetString("CONOMECONTA");
                listaTreinos.Add(treino);
            }
            return listaTreinos;
        }
        public async Task<List<Treino>> BuscarTreinosCapaCT(int codigoCT) {

            var listaTreinos = new List<Treino>();

            string sql = $@"
                        SELECT
	                        TR.TRCODTREINO,
	                        TR.TRNOMETREINO
                        FROM TREINO TR
                        WHERE
                            TR.TRCODCRIADOR = @obj0
            ";
            var parametros = Parametrizar(codigoCT);

            var dr = Query(sql, parametros);

            while (await dr.ReadAsync()) {
                var treino = new Treino();
                treino.Codigo = dr.GetInt32("TRCODTREINO");
                treino.Nome = dr.GetString("TRNOMETREINO");
                listaTreinos.Add(treino);
            }
            return listaTreinos;
        }
        public async Task<Treino> BuscarTreinoBasico(int codigoTreino) {
            string sql = $@"
                        SELECT
	                        TR.TRNOMETREINO
                        FROM TREINO TR
                        WHERE
                            TR.TRCODTREINO = @obj0
            ";
            var parametros = Parametrizar(codigoTreino);

            var dr = Query(sql, parametros);

            if (await dr.ReadAsync()) {
                var treino = new Treino();
                treino.Nome = dr.GetString("TRNOMETREINO");
                return treino;
            }
            return null;
        }
        public async Task<Treino> BuscarDetalhesTreino(int codigoTreino) {

            string sql = $@"
                        SELECT
	                        TRCODTREINO,
                            TRNOMETREINO,
	                        TRDESCRICAOTREINO,
                            TRDATAVENCIMENTO,
                            TRMODALIDADE,
                            TRLIMITEALUNO
                        FROM TREINO
                        WHERE
                            TRCODTREINO = @obj0
            ";
            var parametros = Parametrizar(codigoTreino);

            var dr = Query(sql, parametros);

            if (await dr.ReadAsync()) {
                var treino = new Treino();
                treino.Codigo = dr.GetInt32("TRCODTREINO");
                treino.Nome = dr.GetString("TRNOMETREINO");
                treino.Descricao = dr.GetString("TRDESCRICAOTREINO");
                treino.DataVencimento = dr.GetDateTime("TRDATAVENCIMENTO");
                treino.Modalidade = (ModalidadeTreino) dr.GetByte("TRMODALIDADE");
                treino.LimiteAlunos = dr.GetInt32("TRLIMITEALUNO");
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
                         TRMODALIDADE,
                         TRLIMITEALUNO
                     )
                    OUTPUT INSERTED.TRCODTREINO
                VALUES
                    (
                        @obj0,
                        @obj1,
                        @obj2,
                        @obj3,
                        @obj4,
                        @obj5,
                        @obj6
                    )
            ";

            var parametros = Parametrizar(treino.Nome, treino.Descricao, DateTime.Now, treino.DataVencimento, treino.Criador.Codigo, treino.Modalidade, treino.LimiteAlunos);

            var dr =Query(sql, parametros);

            if (await dr.ReadAsync()) {
                return dr.GetInt32("TRCODTREINO");
            }
            return 0;
        }
        public async Task AtualizarTreino(Treino treino) {
            string sql = @"
                    UPDATE TREINO
                    SET TRNOMETREINO = @obj0,
                        TRDESCRICAOTREINO = @obj1,
                        TRDATAVENCIMENTO = @obj2,
                        TRMODALIDADE = @obj3,
                        TRLIMITEALUNO = @obj4
                    WHERE
                        TRCODTREINO = @obj5
            ";

            var parametros = Parametrizar(treino.Nome, treino.Descricao, treino.DataVencimento, treino.Modalidade, treino.LimiteAlunos, treino.Codigo);

            await NonQuery(sql, parametros);
        }
        public async Task<List<Conta>> BuscarAlunos(int codigoTreino) {

            var alunos = new List<Conta>();

            string sql = @"
                SELECT
	                CO.COCODCONTA,
	                CO.CONOMECONTA,
	                CO.COEMAIL
                FROM TREINOALUNO TA
                INNER JOIN CONTA CO ON CO.COCODCONTA = TA.TACODALUNO
                WHERE
                 TA.TACODTREINO = @obj0"
            ;

            var parametros = Parametrizar(codigoTreino);

            var dr = Query(sql, parametros);

            while (await dr.ReadAsync()) {
                var conta = new Conta();
                conta.Codigo = dr.GetInt32("COCODCONTA");
                conta.Nome = dr.GetString("CONOMECONTA");
                conta.Email = dr.GetString("COEMAIL");
                alunos.Add(conta);
            }
            return alunos;
        }
        public async Task<int> AdicionarAluno(int codigoTreino, string emailAluno) {
            string sql = @"
                    INSERT INTO TREINOALUNO
                     (
                         TACODTREINO,
                         TACODALUNO
                     )
                    OUTPUT INSERTED.TACODALUNO
                    VALUES ( @obj0, (SELECT COCODCONTA FROM CONTA WHERE COEMAIL = @obj1) )
            ";

            var parametros = Parametrizar(codigoTreino, emailAluno);

            var dr = Query(sql, parametros);

            if (await dr.ReadAsync()) {
                return dr.GetInt32("TACODALUNO");
            }
            return 0;
        }
        public async Task DeletarTreino(int codigoTreino) {
            string sql = @"
                    DELETE FROM TREINO
                    WHERE TRCODTREINO = @obj0
            ";
            var parametros = Parametrizar(codigoTreino);

            await NonQuery(sql, parametros);
        }
        public async Task RemoverAluno(int codigoTreino, int codigoConta) {
            string sql = @"
                        DELETE FROM TREINOALUNO
                        WHERE TACODTREINO = @obj0 AND TACODALUNO = @obj1
            ";

            var parametros = Parametrizar(codigoTreino, codigoConta);

            await NonQuery(sql, parametros);
        }
    }
}
