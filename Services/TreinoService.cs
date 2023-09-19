using TreinoSportAPI.MapperNoSQL;
using TreinoSportAPI.Mappers;
using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;

namespace TreinoSportAPI.Services {
    public class TreinoService {

        private readonly TreinoMapper _treinoMapper;
        private readonly TreinoMapperNoSQL _treinoMapperNoSQL;
        private readonly ContaMapper contaMapper;

        public TreinoService(TreinoMapper treinoMapper, TreinoMapperNoSQL treinoMapperNoSQL, ContaMapper contaMapper) {
            _treinoMapper = treinoMapper;
            _treinoMapperNoSQL = treinoMapperNoSQL;
            this.contaMapper = contaMapper;
        }

        public Task<List<Treino>> GetTreinosComoAluno(int codigoUsuario) {
            return _treinoMapper.GetTreinosComoAluno(codigoUsuario);
        }

        public Task<List<Treino>> GetTreinosComoCT(int codigoCT) {
            return _treinoMapper.BuscarTreinosCT(codigoCT);
        }

        public async Task InserirTreino(Treino treino) {
            var codigoTreino = await _treinoMapper.InserirTreino(treino);
            if (codigoTreino == 0 ) {
                throw new Exception("Erro ao inserir treino");
            }
            await InserirHorarios(codigoTreino, treino.DatasTreinos);
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
            return _treinoMapperNoSQL.AtualizarDiasHorarios(diaDaSemanaDTO);
        }

        public async Task<Treino> BuscarDetalhesTreino(int codigoTreino) {
            var treino = await _treinoMapper.BuscarDetalhesTreino(codigoTreino);
            treino.DatasTreinos = await _treinoMapperNoSQL.BuscarHorarios(codigoTreino);
            return treino;
        }

        public async Task AtualizarTreino(Treino treino) {
            await _treinoMapper.AtualizarTreino(treino);
            await AtualizarHorarios(treino.Codigo, treino.DatasTreinos);
        }

        public async Task<Treino> BuscarTreinoBasico(int codigoTreino) {
            var treino = await _treinoMapper.BuscarTreinoBasico(codigoTreino);
            treino.DatasTreinos = await _treinoMapperNoSQL.BuscarHorarios(codigoTreino);
            return treino;
        }

        public async Task<List<Treino>> BuscarTreinosParaGerenciar(int codigoCT) {
            var treinos = await _treinoMapper.BuscarTreinosCT(codigoCT);
            foreach (var treino in treinos) {
                treino.DatasTreinos = await _treinoMapperNoSQL.BuscarHorarios(treino.Codigo);
            }
            return treinos;
        }
        public Task<List<Conta>> BuscarAlunos(int codigoTreino) {
            return _treinoMapper.BuscarAlunos(codigoTreino);
        }
        public async Task AdicionarAluno(int codigoTreino, string emailAluno) {
            var emailExiste = await contaMapper.ChecarEmail(emailAluno);
            if (!emailExiste) {
                throw new Exception("Email não existe.");
            }
            await _treinoMapper.AdicionarAluno(codigoTreino, emailAluno);
        }
    }
}
