using TreinoSportAPI.Models;

namespace TreinoSportAPI.Services.Interfaces {
    public interface ITreinoService {
        Task<List<Treino>> GetTreinosComoAluno(int codigoUsuario);
        Task<List<Treino>> GetTreinosComoCT(int codigoCT);
        Task InserirTreino(Treino treino);
        Task InserirHorarios(int codigoTreino, List<DiaDaSemana> dias);
        Task<List<DiaDaSemana>> BuscarHorarios(int codigoTreino);
        Task AtualizarHorarios(int codigoTreino, List<DiaDaSemana> dias);
        Task<Treino> BuscarDetalhesTreino(int codigoTreino);
        Task<Treino> BuscarDetalhesTreinoBasico(int codigoTreino);
        Task AtualizarTreino(Treino treino);
        Task<Treino> BuscarTreinoBasico(int codigoTreino);
        Task<List<Treino>> BuscarTreinosComCores(int codigoConta, bool isCT);
        Task<List<Conta>> BuscarAlunos(int codigoTreino);
        Task<Conta> AdicionarAluno(int codigoTreino, string emailAluno);
        Task DeletarTreino(int codigoTreino);
        Task RemoverAluno(int codigoTreino, int codigoConta);
        Task InserirAlunoHorario(int codigoTreino, int codigoDia, int codigoHorario, int codigoAluno, List<DiaDaSemana> diasDaSemana);
        Task<List<Conta>> BuscarAlunosPresentes(int codigoTreino, int codigoDia, int codigoHorario);
        Task RemoverAlunoHorario(int codigoTreino, int codigoDia, int codigoHorario, int codigoAluno, List<DiaDaSemana> diasDaSemana);
    }
}
