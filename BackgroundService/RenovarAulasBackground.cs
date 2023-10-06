
using TreinoSportAPI.MapperNoSQL;
using TreinoSportAPI.Services;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.BackgroundService {
    public class RenovarAulasBackground : Microsoft.Extensions.Hosting.BackgroundService {
        private TreinoMapperNoSQL _treinoNoSQL;
        public RenovarAulasBackground() {
            _treinoNoSQL = new();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour <= 5) {
                    await ReiniciarPresencas();
                    await Task.Delay((int)UtilEnvironment.ConverteHorasToMs(24), stoppingToken);
                }
                else {
                    var horasParaMeiaNoite = 24 - DateTime.Now.Hour;
                    await Task.Delay((int)UtilEnvironment.ConverteHorasToMs(horasParaMeiaNoite+1), stoppingToken);
                }
            }
        }

        private async Task ReiniciarPresencas() {
            var listaDTO = await _treinoNoSQL.BuscarTodosHorarios();
            var ontem = DateTime.Now.AddDays(-1);
            foreach (var treino in listaDTO) {

                foreach (var diaDaSemana in treino.DatasTreinos) {

                    if (diaDaSemana.Dia == ontem.DayOfWeek) {

                        foreach (var horario in diaDaSemana.Horarios)
                        {
                            horario.AlunosPresentes.Clear();
                        }
                    }
                }
            }
        }
    }
}
