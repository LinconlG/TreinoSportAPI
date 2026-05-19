using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;

namespace TreinoSportAPI.Mappers.Interfaces {
    public interface ITreinoMapper {
        Task<List<Treino>> GetTreinosComoAluno(int codigoUsuario);
        Task<List<Treino>> BuscarTreinosCapaCT(int codigoCT);
        Task<Treino> BuscarTreinoBasico(int codigoTreino);
        Task<Treino> BuscarDetalhesTreino(int codigoTreino);
        Task<int> InserirTreino(Treino treino);
        Task AtualizarTreino(Treino treino);
        Task<List<Conta>> BuscarAlunos(int codigoTreino);
        Task<int> AdicionarAluno(int codigoTreino, string emailAluno);
        Task DeletarTreino(int codigoTreino);
        Task DeletarAlunosTreino(int codigoTreino);
        Task RemoverAluno(int codigoTreino, int codigoConta);
    }
}
