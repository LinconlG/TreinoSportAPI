
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreinoSportAPI.Models;
using TreinoSportAPI.Services;
using TreinoSportAPI.Services.Interfaces;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TreinoController : ControllerBase {

        private readonly ITreinoService _treinoService;

        public TreinoController(ITreinoService treinoService) {
            _treinoService = treinoService;
        }

        /// <summary>Retorna todos os treinos nos quais o usuário está inscrito como aluno.</summary>
        [HttpGet("aluno/todos")]
        public async Task<ActionResult<List<Treino>>> GetTreinosComoAluno([FromQuery(Name = "codigoUsuario")] int codigoUsuario) {
            var lista = await _treinoService.GetTreinosComoAluno(codigoUsuario);
            return Ok(lista);
        }

        /// <summary>Retorna todos os treinos criados pelo CT autenticado.</summary>
        [HttpGet("ct/todos")]
        public async Task<ActionResult<List<Treino>>> GetTreinosComoCT() {
            var codigoCT = this.ObterCodigoConta();
            var lista = await _treinoService.GetTreinosComoCT(codigoCT);
            return Ok(lista);
        }

        /// <summary>Retorna os horários de um treino.</summary>
        [HttpGet("ct/horarios")]
        public async Task<ActionResult<List<Treino>>> GetHorarios([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            var listaHorarios = await _treinoService.BuscarHorarios(codigoTreino);
            return Ok(listaHorarios);
        }

        /// <summary>Atualiza os horários de um treino.</summary>
        [HttpPatch("ct/horarios")]
        public async Task<ActionResult> PatchHorarios([FromQuery(Name = "codigoTreino")] int codigoTreino, [FromBody] List<DiaDaSemana> dias) {
            await _treinoService.AtualizarHorarios(codigoTreino, dias);
            return Ok();
        }

        /// <summary>Cria um novo treino para o CT autenticado.</summary>
        [HttpPut("ct/criar")]
        public async Task<ActionResult> PutTreino([FromBody] Treino treino) {
            treino.Criador = new() {
                Codigo = this.ObterCodigoConta()
            };
            await _treinoService.InserirTreino(treino);
            return Ok();
        }

        /// <summary>Atualiza os detalhes de um treino.</summary>
        [HttpPatch("ct/detalhes")]
        public async Task<ActionResult> PatchDetalhes([FromBody] Treino treino) {
            await _treinoService.AtualizarTreino(treino);
            return Ok();
        }

        /// <summary>Retorna os detalhes completos de um treino incluindo horários.</summary>
        [HttpGet("ct/detalhes")]
        public async Task<ActionResult<Treino>> GetDetalhesTreino([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            var treino = await _treinoService.BuscarDetalhesTreino(codigoTreino);
            return Ok(treino);
        }

        /// <summary>Retorna os detalhes básicos de um treino sem horários do MongoDB.</summary>
        [HttpGet("ct/detalhes/basico")]
        public async Task<ActionResult<Treino>> GetDetalhesTreinoBasico([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            var treino = await _treinoService.BuscarDetalhesTreinoBasico(codigoTreino);
            return Ok(treino);
        }

        /// <summary>Deleta um treino e todos os seus dados associados.</summary>
        [HttpDelete("ct/detalhes")]
        public async Task<ActionResult<Treino>> DeleteTreino([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            await _treinoService.DeletarTreino(codigoTreino);
            return Ok();
        }

        /// <summary>Retorna a lista de treinos com cores e horários para gerenciamento.</summary>
        [HttpGet("gerenciamento/lista")]
        public async Task<ActionResult<List<Treino>>> GetTreinosParaGerenciar([FromQuery(Name = "codigoConta")] int codigoConta, [FromQuery(Name = "isCT")] bool isCT) {
            var treinos = await _treinoService.BuscarTreinosComCores(codigoConta, isCT);
            return Ok(treinos);
        }

        /// <summary>Retorna informações básicas de um treino específico com horários.</summary>
        [HttpGet("gerenciamento/especifico")]
        public async Task<ActionResult<Treino>> GetTreinoBasico([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            var treino = await _treinoService.BuscarTreinoBasico(codigoTreino);
            return Ok(treino);
        }

        /// <summary>Retorna a lista de alunos inscritos em um treino.</summary>
        [HttpGet("alunos")]
        public async Task<ActionResult<List<Conta>>> GetAlunos([FromQuery(Name = "codigoTreino")] int codigoTreino) {
            var alunos = await _treinoService.BuscarAlunos(codigoTreino);
            return Ok(alunos);
        }

        /// <summary>Adiciona um aluno ao treino pelo email.</summary>
        [HttpPut("alunos")]
        public async Task<ActionResult<Conta>> PutAluno([FromQuery(Name = "codigoTreino")] int codigoTreino, [FromQuery(Name = "emailAluno")] string emailAluno) {
            var alunoInserido = await _treinoService.AdicionarAluno(codigoTreino, emailAluno);
            return Ok(alunoInserido);
        }

        /// <summary>Remove um aluno de um treino.</summary>
        [HttpDelete("alunos")]
        public async Task<ActionResult> DeleteAluno([FromQuery(Name = "codigoTreino")] int codigoTreino, [FromQuery(Name = "codigoConta")] int codigoConta) {
            await _treinoService.RemoverAluno(codigoTreino, codigoConta);
            return Ok();
        }

        /// <summary>Marca a presença de um aluno em um horário específico do treino.</summary>
        [HttpPatch("aluno/presenca/marcar")]
        public async Task<ActionResult> PatchInserirAlunoHorario(
            [FromQuery(Name = "codigoTreino")] int codigoTreino,
            [FromQuery(Name = "codigoDia")] int codigoDia,
            [FromQuery(Name = "codigoHorario")] int codigoHorario,
            [FromQuery(Name = "codigoAluno")] int codigoAluno,
            [FromBody] List<DiaDaSemana> diasDaSemana) {
            await _treinoService.InserirAlunoHorario(codigoTreino, codigoDia, codigoHorario, codigoAluno, diasDaSemana);
            return Ok();
        }

        /// <summary>Remove a presença de um aluno em um horário específico do treino.</summary>
        [HttpPatch("aluno/presenca/remover")]
        public async Task<ActionResult> PatchDeletarAlunoHorario(
            [FromQuery(Name = "codigoTreino")] int codigoTreino,
            [FromQuery(Name = "codigoDia")] int codigoDia,
            [FromQuery(Name = "codigoHorario")] int codigoHorario,
            [FromQuery(Name = "codigoAluno")] int codigoAluno,
            [FromBody] List<DiaDaSemana> diasDaSemana) {
            await _treinoService.RemoverAlunoHorario(codigoTreino, codigoDia, codigoHorario, codigoAluno, diasDaSemana);
            return Ok();
        }

        /// <summary>Retorna a lista de alunos presentes em um horário específico do treino.</summary>
        [HttpGet("presentes")]
        public async Task<ActionResult<List<Conta>>> GetAlunosPresentes([FromQuery(Name = "codigoTreino")] int codigoTreino, 
            [FromQuery(Name = "codigoDia")] int codigoDia, 
            [FromQuery(Name = "codigoHorario")] int codigoHorario) {
            var alunos = await _treinoService.BuscarAlunosPresentes(codigoTreino, codigoDia, codigoHorario);
            return Ok(alunos);
        }
    }
}
