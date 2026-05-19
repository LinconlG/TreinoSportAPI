using TreinoSportAPI.Mappers.NoSQL;
using TreinoSportAPI.Mappers;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Services {
    public class TreinoService : ITreinoService {

        private readonly ITreinoMapper _treinoMapper;
        private readonly ITreinoMapperNoSQL _treinoMapperNoSQL;
        private readonly IContaMapper contaMapper;

        public TreinoService(ITreinoMapper treinoMapper, ITreinoMapperNoSQL treinoMapperNoSQL, IContaMapper contaMapper) {
            _treinoMapper = treinoMapper;
            _treinoMapperNoSQL = treinoMapperNoSQL;
            this.contaMapper = contaMapper;
        }

        /// <summary>
        /// Retorna os treinos nos quais o usuário está inscrito como aluno.
        /// </summary>
        public Task<List<Treino>> GetTreinosComoAluno(int codigoUsuario) {
            return _treinoMapper.GetTreinosComoAluno(codigoUsuario);
        }

        /// <summary>
        /// Retorna os treinos criados pelo Centro de Treinamento informado.
        /// </summary>
        public Task<List<Treino>> GetTreinosComoCT(int codigoCT) {
            return _treinoMapper.BuscarTreinosCapaCT(codigoCT);
        }

        /// <summary>
        /// Insere um novo treino e seus horários no banco de dados.
        /// </summary>
        public async Task InserirTreino(Treino treino) {
            ReassignHorarioCodigos(treino.DatasTreinos);
            var codigoTreino = await _treinoMapper.InserirTreino(treino);
            if (codigoTreino == 0 ) {
                throw new Exception("Erro ao inserir treino");
            }
            await InserirHorarios(codigoTreino, treino.DatasTreinos);
        }

        /// <summary>
        /// Insere os horários de um treino no MongoDB.
        /// </summary>
        public Task InserirHorarios(int codigoTreino, List<DiaDaSemana> dias) {
            var diaDaSemanaDTO = new DiaDaSemanaDTO();
            diaDaSemanaDTO.CodigoTreino = codigoTreino;
            diaDaSemanaDTO.DatasTreinos = dias.OrderBy(dia => dia.Dia).ToList();
            return _treinoMapperNoSQL.InserirHorarios(diaDaSemanaDTO);
        }

        /// <summary>
        /// Busca os horários de um treino pelo código do treino.
        /// </summary>
        public Task<List<DiaDaSemana>> BuscarHorarios(int codigoTreino) {
            return _treinoMapperNoSQL.BuscarHorarios(codigoTreino);
        }

        /// <summary>
        /// Atualiza os horários de um treino no MongoDB.
        /// </summary>
        public Task AtualizarHorarios(int codigoTreino, List<DiaDaSemana> dias) {
            ReassignHorarioCodigos(dias);
            var diaDaSemanaDTO = new DiaDaSemanaDTO();
            diaDaSemanaDTO.CodigoTreino = codigoTreino;
            diaDaSemanaDTO.DatasTreinos = dias;
            return _treinoMapperNoSQL.AtualizarDiasHorarios(diaDaSemanaDTO);
        }

        /// <summary>
        /// Reatribui os códigos dos horários de forma sequencial para evitar duplicatas.
        /// </summary>
        private static void ReassignHorarioCodigos(List<DiaDaSemana> dias) {
            int horarioId = 1;
            foreach (var dia in dias) {
                foreach (var horario in dia.Horarios) {
                    horario.Codigo = horarioId++;
                }
            }
        }

        /// <summary>
        /// Busca os detalhes completos de um treino incluindo horários do MongoDB.
        /// </summary>
        public async Task<Treino> BuscarDetalhesTreino(int codigoTreino) {
            var treino = await _treinoMapper.BuscarDetalhesTreino(codigoTreino);
            treino.DatasTreinos = await _treinoMapperNoSQL.BuscarHorarios(codigoTreino);
            return treino;
        }
        /// <summary>
        /// Busca os detalhes básicos de um treino sem horários do MongoDB.
        /// </summary>
        public async Task<Treino> BuscarDetalhesTreinoBasico(int codigoTreino) {
            var treino = await _treinoMapper.BuscarDetalhesTreino(codigoTreino);
            return treino;
        }

        /// <summary>
        /// Atualiza os dados do treino e seus horários.
        /// </summary>
        public async Task AtualizarTreino(Treino treino) {
            await _treinoMapper.AtualizarTreino(treino);
            await AtualizarHorarios(treino.Codigo, treino.DatasTreinos);
        }

        /// <summary>
        /// Busca informações básicas de um treino com seus horários do MongoDB.
        /// </summary>
        public async Task<Treino> BuscarTreinoBasico(int codigoTreino) {
            var treino = await _treinoMapper.BuscarTreinoBasico(codigoTreino);
            treino.DatasTreinos = await _treinoMapperNoSQL.BuscarHorarios(codigoTreino);
            return treino;
        }

        /// <summary>
        /// Busca os treinos de uma conta com as cores e horários associados.
        /// </summary>
        public async Task<List<Treino>> BuscarTreinosComCores(int codigoConta, bool isCT) {
            var treinos = new List<Treino>();
            if (isCT) {
                treinos = await _treinoMapper.BuscarTreinosCapaCT(codigoConta);
            }
            else {
                treinos = await _treinoMapper.GetTreinosComoAluno(codigoConta);
            }

            foreach (var treino in treinos) {
                treino.DatasTreinos = await _treinoMapperNoSQL.BuscarHorarios(treino.Codigo);
            }
            return treinos;
        }
        /// <summary>
        /// Retorna a lista de alunos inscritos em um treino.
        /// </summary>
        public Task<List<Conta>> BuscarAlunos(int codigoTreino) {
            return _treinoMapper.BuscarAlunos(codigoTreino);
        }
        /// <summary>
        /// Adiciona um aluno ao treino pelo email, validando se o email existe.
        /// </summary>
        public async Task<Conta> AdicionarAluno(int codigoTreino, string emailAluno) {
            var emailExiste = await contaMapper.ChecarEmail(emailAluno);
            if (!emailExiste) {
                throw new APIException("Email não existe.", true);
            }
            var codigoAluno = await _treinoMapper.AdicionarAluno(codigoTreino, emailAluno);
            var conta = await contaMapper.BuscarConta(codigoAluno);
            return conta;
        }
        /// <summary>
        /// Deleta um treino e todos os seus alunos associados.
        /// </summary>
        public async Task DeletarTreino(int codigoTreino) {
            await _treinoMapper.DeletarAlunosTreino(codigoTreino);
            await _treinoMapper.DeletarTreino(codigoTreino);
            await _treinoMapperNoSQL.DeletarHorarios(codigoTreino);
        }
        /// <summary>
        /// Remove um aluno de um treino pelo código da conta.
        /// </summary>
        public async Task RemoverAluno(int codigoTreino, int codigoConta) {
            await _treinoMapper.RemoverAluno(codigoTreino, codigoConta);
        }
        /// <summary>
        /// Insere um aluno como presente em um horário específico do treino.
        /// </summary>
        public async Task InserirAlunoHorario(int codigoTreino, int codigoDia, int codigoHorario, int codigoAluno, List<DiaDaSemana> diasDaSemana) {
            var aluno = await contaMapper.BuscarConta(codigoAluno);

            foreach (var dia in diasDaSemana) {
                if ((int)dia.Dia == codigoDia) {
                    foreach (var horario in dia.Horarios) {
                        if (horario.Codigo == codigoHorario) {
                            horario.AlunosPresentes.Add(aluno);
                        }
                    }
                }
            }
            await AtualizarHorarios(codigoTreino, diasDaSemana);
        }
        /// <summary>
        /// Retorna a lista de alunos presentes em um horário específico do treino.
        /// </summary>
        public async Task<List<Conta>> BuscarAlunosPresentes(int codigoTreino, int codigoDia, int codigoHorario) {
            var treino = await _treinoMapperNoSQL.BuscarAlunosPresentes(codigoTreino);
            foreach (var data in treino.DatasTreinos) {

                if (data.Dia == (DayOfWeek)codigoDia) {

                    foreach (var horario in data.Horarios) {

                        if (horario.Codigo == codigoHorario) {
                            return horario.AlunosPresentes;
                        }
                    }
                }
            }
            throw new APIException("Erro ao buscar alunos presentes, recrie o treino ou entre em contato.", true);
        }
        /// <summary>
        /// Remove um aluno da lista de presentes em um horário específico do treino.
        /// </summary>
        public async Task RemoverAlunoHorario(int codigoTreino, int codigoDia, int codigoHorario, int codigoAluno, List<DiaDaSemana> diasDaSemana) {

            foreach (var dia in diasDaSemana) {
                if ((int)dia.Dia == codigoDia) {
                    foreach (var horario in dia.Horarios) {
                        if (horario.Codigo == codigoHorario) {
                            horario.AlunosPresentes.RemoveAll(aluno => aluno.Codigo == codigoAluno);
                        }
                    }
                }
            }
            await AtualizarHorarios(codigoTreino, diasDaSemana);
        }

    }
}
