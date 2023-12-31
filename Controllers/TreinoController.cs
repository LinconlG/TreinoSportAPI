﻿
using Microsoft.AspNetCore.Mvc;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Utilities;

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
                return this.InternalServerError(e.Message, e.IsPublicMessageCheck());
            }
        }

        [HttpGet("ct/detalhes/basico")]
        public async Task<ActionResult<Treino>> GetDetalhesTreinoBasico([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            try {
                var treino = await _treinoService.BuscarDetalhesTreinoBasico(codigoTreino);
                return Ok(treino);
            }
            catch (Exception e) {
                return this.InternalServerError(e.Message, e.IsPublicMessageCheck());
            }
        }

        [HttpDelete("ct/detalhes")]
        public async Task<ActionResult<Treino>> DeleteTreino([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            try {
                await _treinoService.DeletarTreino(codigoTreino);
                return Ok();
            }
            catch (Exception e) {
                return UtilEnvironment.InternalServerError(this, e.Message, UtilEnvironment.IsPublicMessageCheck(e));//===========================================================
            }
        }

        [HttpGet("gerenciamento/lista")]
        public async Task<ActionResult<List<Treino>>> GetTreinosParaGerenciar([FromQuery(Name = "codigoConta")] int codigoConta, [FromQuery(Name = "isCT")] bool isCT) {
            try {
                var treinos = await _treinoService.BuscarTreinosComCores(codigoConta, isCT);
                return Ok(treinos);
            }
            catch (Exception e) {
                return this.InternalServerError(e.Message, e.IsPublicMessageCheck());
            }
        }

        [HttpGet("gerenciamento/especifico")]
        public async Task<ActionResult<Treino>> GetTreinoBasico([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            try {
                var treino = await _treinoService.BuscarTreinoBasico(codigoTreino);
                return Ok(treino);
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [HttpGet("alunos")]
        public async Task<ActionResult<List<Conta>>> GetAlunos([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            try {
                var alunos = await _treinoService.BuscarAlunos(codigoTreino);
                return Ok(alunos);
            }
            catch (Exception e) {
                return this.InternalServerError(e.Message, e.IsPublicMessageCheck());
            }
        }

        [HttpPut("alunos")]
        public async Task<ActionResult<Conta>> PutAluno([FromQuery(Name = "codigoTreino")] int codigoTreino, [FromQuery(Name = "emailAluno")] string emailAluno) {
            try {
                var alunoInserido = await _treinoService.AdicionarAluno(codigoTreino, emailAluno);
                return Ok(alunoInserido);
            }
            catch (Exception e) {
                return UtilEnvironment.InternalServerError(this, e.Message, UtilEnvironment.IsPublicMessageCheck(e));
            }
        }

        [HttpDelete("alunos")]
        public async Task<ActionResult> DeleteAluno([FromQuery(Name = "codigoTreino")] int codigoTreino, [FromQuery(Name = "codigoConta")] int codigoConta) {
            try {
                await _treinoService.RemoverAluno(codigoTreino, codigoConta);
                return Ok();
            }
            catch (Exception e) {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        [HttpPatch("aluno/presenca/marcar")]
        public async Task<ActionResult> PatchInserirAlunoHorario(
            [FromQuery(Name = "codigoTreino")] int codigoTreino,
            [FromQuery(Name = "codigoDia")] int codigoDia,
            [FromQuery(Name = "codigoHorario")] int codigoHorario,
            [FromQuery(Name = "codigoAluno")] int codigoAluno,
            [FromBody] List<DiaDaSemana> diasDaSemana) {
            try {
                await _treinoService.InserirAlunoHorario(codigoTreino, codigoDia, codigoHorario, codigoAluno, diasDaSemana);
                return Ok();
            }
            catch (Exception e) {
                return UtilEnvironment.InternalServerError(this, e.Message, UtilEnvironment.IsPublicMessageCheck(e));
            }
        }

        [HttpPatch("aluno/presenca/remover")]
        public async Task<ActionResult> PatchDeletarAlunoHorario(
            [FromQuery(Name = "codigoTreino")] int codigoTreino,
            [FromQuery(Name = "codigoDia")] int codigoDia,
            [FromQuery(Name = "codigoHorario")] int codigoHorario,
            [FromQuery(Name = "codigoAluno")] int codigoAluno,
            [FromBody] List<DiaDaSemana> diasDaSemana) {
            try {
                await _treinoService.RemoverAlunoHorario(codigoTreino, codigoDia, codigoHorario, codigoAluno, diasDaSemana);
                return Ok();
            }
            catch (Exception e) {
                return this.InternalServerError(e.Message, e.IsPublicMessageCheck());
            }
        }

        [HttpGet("presentes")]
        public async Task<ActionResult<List<Conta>>> GetAlunosPresentes([FromQuery(Name = "codigoTreino")] int codigoTreino, 
            [FromQuery(Name = "codigoDia")] int codigoDia, 
            [FromQuery(Name = "codigoHorario")] int codigoHorario) {
            try {
                var alunos = await _treinoService.BuscarAlunosPresentes(codigoTreino, codigoDia, codigoHorario);
                return Ok(alunos);
            }
            catch (Exception e) {
                return this.InternalServerError(e.Message, e.IsPublicMessageCheck());
            }
        }
    }
}
