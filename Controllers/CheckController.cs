using Microsoft.AspNetCore.Mvc;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;

namespace TreinoSportAPI.Controllers {

    [ApiController]
    [Route("api/[controller]")]
    public class CheckController : Controller {

        /// <summary>Verifica se a API está online.</summary>
        [HttpGet]
        public ActionResult Check() {
            return Ok();
        }
    }
}
