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
        public async Task<ActionResult> PatchHorarios([FromQuery(Name = "codigoTreino")] int codigoTreino, [FromBody] List<DiaDaSemana> diasDaSemana) {
            try {
                await _treinoService.AtualizarHorarios(codigoTreino, diasDaSemana);
                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [HttpGet("/detalhes")]
        public async Task<ActionResult<Treino>> GetDetalhesTreino([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            try {
                var treino = await _treinoService.BuscarDetalhesTreino(codigoTreino);
                return Ok(treino);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
