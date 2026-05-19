using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;

namespace TreinoSportAPI.Mappers.Interfaces {
    public interface ITreinoMapperNoSQL {
        Task InserirHorarios(DiaDaSemanaDTO diaDaSemanaDTO);
        Task<List<DiaDaSemana>> BuscarHorarios(int codigoTreino);
        Task<DiaDaSemanaDTO> BuscarAlunosPresentes(int codigoTreino);
        Task<List<DiaDaSemanaDTO>> BuscarTodosHorarios();
        Task AtualizarDiasHorarios(DiaDaSemanaDTO diaDaSemanaDTO);
        Task DeletarHorarios(int codigoTreino);
    }
}
