using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;

namespace TreinoSportAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class TreinoController : ControllerBase {

        private readonly TreinoService _treinoService;

        public TreinoController(TreinoService treinoService) {
            _treinoService = treinoService;
        }

        [HttpGet("aluno/todos")]
        public async Task<ActionResult<List<Treino>>> GetTreinosComoAluno([FromQuery(Name = "codigoUsuario")] int codigoUsuario) {
            try {
                var lista = await _treinoService.GetTreinosComoAluno(codigoUsuario);
                return Ok(lista);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [HttpGet("ct/todos")]
        public async Task<ActionResult<List<Treino>>> GetTreinosComoCT([FromQuery(Name = "codigoCT")] int codigoCT) {
            try {
                var lista = await _treinoService.GetTreinosComoCT(codigoCT);
                return Ok(lista);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [HttpGet("ct/horarios")]
        public async Task<ActionResult<List<Treino>>> GetHorarios([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            try {
                var listaHorarios = await _treinoService.BuscarHorarios(codigoTreino);
                return Ok(listaHorarios);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [HttpPatch("ct/horarios")]
        public async Task<ActionResult> PatchHorarios([FromQuery(Name = "codigoTreino")] int codigoTreino, [FromBody] List<DiaDaSemana> dias) {
            try {
                await _treinoService.AtualizarHorarios(codigoTreino, dias);
                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [HttpPut("ct/criar")]
        public async Task<ActionResult> PutTreino([FromBody] Treino treino) {
            try {
                await _treinoService.InserirTreino(treino);
                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [HttpPatch("ct/detalhes")]
        public async Task<ActionResult> PatchDetalhes([FromBody] Treino treino) {
            try {
                await _treinoService.AtualizarTreino(treino);
                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [HttpGet("ct/detalhes")]
        public async Task<ActionResult<Treino>> GetDetalhesTreino([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            try {
                var treino = await _treinoService.BuscarDetalhesTreino(codigoTreino);
                return Ok(treino);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [HttpGet("gerenciamento/lista")]
        public async Task<ActionResult<List<Treino>>> GetTreinosParaGerenciar([FromQuery(Name = "codigoCT")] int codigoCT) {
            try {
                var treinos = await _treinoService.BuscarTreinosParaGerenciar(codigoCT);
                return Ok(treinos);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException); //fazer retornar 500 e retornar um objeto feito para erros
            }
        }

        [HttpGet("gerenciamento/especifico")]
        public async Task<ActionResult<Treino>> GetTreinoBasico([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            try {
                var treino = await _treinoService.BuscarTreinoBasico(codigoTreino);
                return Ok(treino);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException); //fazer retornar 500 e retornar um objeto feito para erros
            }
        }

        [HttpGet("alunos")]
        public async Task<ActionResult<List<Conta>>> GetAlunos([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            try {
                var alunos = await _treinoService.BuscarAlunos(codigoTreino);
                return Ok(alunos);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException); //fazer retornar 500 e retornar um objeto feito para erros
            }
        }

        [HttpPut("alunos")]
        public async Task<ActionResult> PutAlunos([FromQuery(Name = "codigoTreino")] int codigoTreino, [FromQuery(Name = "emailAluno")] string emailAluno) {
            try {
                await _treinoService.AdicionarAluno(codigoTreino, emailAluno);
                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException); //fazer retornar 500 e retornar um objeto feito para erros
            }
        }
    }
}
