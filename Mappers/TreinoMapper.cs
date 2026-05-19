using Dapper;
using TreinoSportAPI.Mappers.Connection;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;
using TreinoSportAPI.Models.Enums;

namespace TreinoSportAPI.Mappers {
    public class TreinoMapper : ITreinoMapper {

        private readonly SqlConnectionFactory _factory;

        public TreinoMapper(SqlConnectionFactory factory) {
            _factory = factory;
        }

        /// <summary>
        /// Retorna os treinos nos quais o usuário está inscrito como aluno.
        /// </summary>
        public async Task<List<Treino>> GetTreinosComoAluno(int codigoUsuario) {
            const string sql = @"SELECT TR.TRCODTREINO AS Codigo, TR.TRNOMETREINO AS Nome,
                                        TR.TRDESCRICAOTREINO AS Descricao, CO.CONOMECONTA AS CriadorNome
                                 FROM TREINO TR
                                 INNER JOIN CONTA CO ON CO.COCODCONTA = TR.TRCODCRIADOR
                                 INNER JOIN TREINOALUNO TA ON TA.TACODALUNO = @CodigoUsuario AND TA.TACODTREINO = TR.TRCODTREINO";
            using var conn = _factory.CreateConnection();
            var rows = await conn.QueryAsync(sql, new { CodigoUsuario = codigoUsuario });
            return rows.Select(r => new Treino {
                Codigo = (int)r.Codigo,
                Nome = (string)r.Nome,
                Descricao = (string)r.Descricao,
                Criador = new Conta { Nome = (string)r.CriadorNome }
            }).ToList();
        }

        /// <summary>
        /// Retorna os treinos criados pelo Centro de Treinamento informado.
        /// </summary>
        public async Task<List<Treino>> BuscarTreinosCapaCT(int codigoCT) {
            const string sql = @"SELECT TR.TRCODTREINO AS Codigo, TR.TRNOMETREINO AS Nome, TR.TRMODALIDADE AS Modalidade
                                 FROM TREINO TR WHERE TR.TRCODCRIADOR = @CodigoCT";
            using var conn = _factory.CreateConnection();
            var rows = await conn.QueryAsync(sql, new { CodigoCT = codigoCT });
            return rows.Select(r => new Treino {
                Codigo = (int)r.Codigo,
                Nome = (string)r.Nome,
                Modalidade = (ModalidadeTreino)(byte)r.Modalidade
            }).ToList();
        }

        /// <summary>
        /// Busca informações básicas de um treino pelo código.
        /// </summary>
        public async Task<Treino> BuscarTreinoBasico(int codigoTreino) {
            const string sql = "SELECT TR.TRNOMETREINO AS Nome FROM TREINO TR WHERE TR.TRCODTREINO = @CodigoTreino";
            using var conn = _factory.CreateConnection();
            var row = await conn.QueryFirstOrDefaultAsync(sql, new { CodigoTreino = codigoTreino });
            if (row == null) return null;
            return new Treino { Nome = (string)row.Nome };
        }

        /// <summary>
        /// Busca os detalhes completos de um treino pelo código.
        /// </summary>
        public async Task<Treino> BuscarDetalhesTreino(int codigoTreino) {
            const string sql = @"SELECT TRCODTREINO AS Codigo, TRNOMETREINO AS Nome, TRDESCRICAOTREINO AS Descricao,
                                        TRDATAVENCIMENTO AS DataVencimento, TRMODALIDADE AS Modalidade, TRLIMITEALUNO AS LimiteAlunos
                                 FROM TREINO WHERE TRCODTREINO = @CodigoTreino";
            using var conn = _factory.CreateConnection();
            var row = await conn.QueryFirstOrDefaultAsync(sql, new { CodigoTreino = codigoTreino });
            if (row == null) return new Treino();
            return new Treino {
                Codigo = (int)row.Codigo,
                Nome = (string)row.Nome,
                Descricao = (string)row.Descricao,
                DataVencimento = (DateTime)row.DataVencimento,
                Modalidade = (ModalidadeTreino)(byte)row.Modalidade,
                LimiteAlunos = (int)row.LimiteAlunos
            };
        }

        /// <summary>
        /// Insere um novo treino no banco de dados e retorna o código gerado.
        /// </summary>
        public async Task<int> InserirTreino(Treino treino) {
            const string sql = @"INSERT INTO TREINO (TRNOMETREINO, TRDESCRICAOTREINO, TRDATACRIACAO, TRDATAVENCIMENTO, TRCODCRIADOR, TRMODALIDADE, TRLIMITEALUNO)
                                 OUTPUT INSERTED.TRCODTREINO
                                 VALUES (@Nome, @Descricao, @DataCriacao, @DataVencimento, @CodigoCriador, @Modalidade, @LimiteAlunos)";
            using var conn = _factory.CreateConnection();
            return await conn.QuerySingleAsync<int>(sql, new {
                treino.Nome,
                treino.Descricao,
                DataCriacao = DateTime.Now,
                treino.DataVencimento,
                CodigoCriador = treino.Criador.Codigo,
                treino.Modalidade,
                treino.LimiteAlunos
            });
        }

        /// <summary>
        /// Atualiza os dados de um treino existente no banco de dados.
        /// </summary>
        public async Task AtualizarTreino(Treino treino) {
            const string sql = @"UPDATE TREINO SET TRNOMETREINO = @Nome, TRDESCRICAOTREINO = @Descricao,
                                        TRDATAVENCIMENTO = @DataVencimento, TRMODALIDADE = @Modalidade, TRLIMITEALUNO = @LimiteAlunos
                                 WHERE TRCODTREINO = @Codigo";
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new { treino.Nome, treino.Descricao, treino.DataVencimento, treino.Modalidade, treino.LimiteAlunos, treino.Codigo });
        }

        /// <summary>
        /// Retorna a lista de alunos inscritos em um treino.
        /// </summary>
        public async Task<List<Conta>> BuscarAlunos(int codigoTreino) {
            const string sql = @"SELECT CO.COCODCONTA AS Codigo, CO.CONOMECONTA AS Nome, CO.COEMAIL AS Email
                                 FROM TREINOALUNO TA
                                 INNER JOIN CONTA CO ON CO.COCODCONTA = TA.TACODALUNO
                                 WHERE TA.TACODTREINO = @CodigoTreino";
            using var conn = _factory.CreateConnection();
            var result = await conn.QueryAsync<Conta>(sql, new { CodigoTreino = codigoTreino });
            return result.ToList();
        }

        /// <summary>
        /// Adiciona um aluno ao treino pelo email e retorna o código do aluno.
        /// </summary>
        public async Task<int> AdicionarAluno(int codigoTreino, string emailAluno) {
            const string sql = @"INSERT INTO TREINOALUNO (TACODTREINO, TACODALUNO)
                                 OUTPUT INSERTED.TACODALUNO
                                 VALUES (@CodigoTreino, (SELECT COCODCONTA FROM CONTA WHERE COEMAIL = @EmailAluno))";
            using var conn = _factory.CreateConnection();
            return await conn.QuerySingleAsync<int>(sql, new { CodigoTreino = codigoTreino, EmailAluno = emailAluno });
        }

        /// <summary>
        /// Deleta um treino do banco de dados pelo código.
        /// </summary>
        public async Task DeletarTreino(int codigoTreino) {
            const string sql = "DELETE FROM TREINO WHERE TRCODTREINO = @CodigoTreino";
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new { CodigoTreino = codigoTreino });
        }

        /// <summary>
        /// Remove todos os alunos associados a um treino.
        /// </summary>
        public async Task DeletarAlunosTreino(int codigoTreino) {
            const string sql = "DELETE FROM TREINOALUNO WHERE TACODTREINO = @CodigoTreino";
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new { CodigoTreino = codigoTreino });
        }

        /// <summary>
        /// Remove um aluno específico de um treino.
        /// </summary>
        public async Task RemoverAluno(int codigoTreino, int codigoConta) {
            const string sql = "DELETE FROM TREINOALUNO WHERE TACODTREINO = @CodigoTreino AND TACODALUNO = @CodigoConta";
            using var conn = _factory.CreateConnection();
            await conn.ExecuteAsync(sql, new { CodigoTreino = codigoTreino, CodigoConta = codigoConta });
        }
    }
}
