using TreinoSportAPI.Mappers;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Services {
    public class TreinoService {

        private readonly TreinoMapper _treinoMapper;

        public TreinoService(TreinoMapper treinoMapper) {
            _treinoMapper = treinoMapper;
        }

        public async Task<List<Treino>> GetTreinosComoAluno(int codigoUsuario) {
            return await _treinoMapper.GetTreinosComoAluno(codigoUsuario);
        }
    }
}
