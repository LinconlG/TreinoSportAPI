using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using TreinoSportAPI.MapperNoSQL.Connection;
using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;

namespace TreinoSportAPI.MapperNoSQL {
    public class TreinoMapperNoSQL {

        private readonly IMongoCollection<DiaDaSemanaDTO> dataHorarioDB;

        public TreinoMapperNoSQL(MongoDBConnection mongoDBConnection) {
            dataHorarioDB = mongoDBConnection.GetCollection<DiaDaSemanaDTO>("TreinoSport", "DataHorario");
        }

        public Task InserirHorarios(DiaDaSemanaDTO diaDaSemanaDTO) {
            CorrigirTimeZone(diaDaSemanaDTO);
            return dataHorarioDB.InsertOneAsync(diaDaSemanaDTO);
        }

        public async Task<List<DiaDaSemana>> BuscarHorarios(int codigoTreino) {
            var diasDaSemanaDTO = await dataHorarioDB.FindSync(dias => dias.CodigoTreino == codigoTreino).FirstOrDefaultAsync();
            var datasTreinos = diasDaSemanaDTO.DatasTreinos;
            return datasTreinos;
        }

        public async Task AtualizarHorarios(DiaDaSemanaDTO diaDaSemanaDTO) {
            var filtro = Builders<DiaDaSemanaDTO>.Filter.Where(dto => dto.CodigoTreino == diaDaSemanaDTO.CodigoTreino);
            var update = Builders<DiaDaSemanaDTO>.Update.Set(dto => dto.DatasTreinos, diaDaSemanaDTO.DatasTreinos);
            CorrigirTimeZone(diaDaSemanaDTO);
            await dataHorarioDB.UpdateOneAsync(filtro, update);
        }

        private void CorrigirTimeZone(DiaDaSemanaDTO dto) {
            foreach (var dia in dto.DatasTreinos) {
                var horariosCorrigidos = new List<DateTime>();
                foreach (var horario in dia.Horarios) {

                    if (horario.Hour < 2) {
                        var horarioTemp = horario.AddDays(1);
                        horariosCorrigidos.Add(horarioTemp.AddHours(-2));
                        continue;
                    }

                    horariosCorrigidos.Add(horario.AddHours(-2));
                }
                dia.Horarios = horariosCorrigidos;
            }
        }
    }
}
