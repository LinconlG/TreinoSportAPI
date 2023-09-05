using TreinoSportAPI.MapperNoSQL;
using TreinoSportAPI.Mappers;
using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;

namespace TreinoSportAPI.Services {
    public class TreinoService {

        private readonly TreinoMapper _treinoMapper;
        private readonly TreinoMapperNoSQL _treinoMapperNoSQL;

        public TreinoService(TreinoMapper treinoMapper, TreinoMapperNoSQL treinoMapperNoSQL) {
            _treinoMapper = treinoMapper;
            _treinoMapperNoSQL = treinoMapperNoSQL;
        }

        public Task<List<Treino>> GetTreinosComoAluno(int codigoUsuario) {
            return _treinoMapper.GetTreinosComoAluno(codigoUsuario);
        }

        public Task<List<Treino>> GetTreinosComoCT(int codigoCT) {
            return _treinoMapper.GetTreinosComoCT(codigoCT);
        }

        public Task InserirHorarios(int codigoTreino, List<DiaDaSemana> dias) {
            var diaDaSemanaDTO = new DiaDaSemanaDTO();
            diaDaSemanaDTO.CodigoTreino = codigoTreino;
            diaDaSemanaDTO.DatasTreinos = dias;
            return _treinoMapperNoSQL.InserirHorarios(diaDaSemanaDTO);
        }

        public Task<List<DiaDaSemana>> BuscarHorarios(int codigoTreino) {
            return _treinoMapperNoSQL.BuscarHorarios(codigoTreino);
        }

        public Task AtualizarHorarios(int codigoTreino, List<DiaDaSemana> dias) {
            var diaDaSemanaDTO = new DiaDaSemanaDTO();
            diaDaSemanaDTO.CodigoTreino = codigoTreino;
            diaDaSemanaDTO.DatasTreinos = dias;
            return _treinoMapperNoSQL.AtualizarHorarios(diaDaSemanaDTO);
        }

    }
}
