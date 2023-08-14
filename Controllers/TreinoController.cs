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

        [HttpGet("todos")]
        public async Task<ActionResult<List<Treino>>> GetTreinosAluno([FromQuery(Name = "codigoUsuario")] int codigoUsuario) {
            try {
                var lista = await _treinoService.GetTreinosAluno(codigoUsuario);
                return Ok(lista);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
