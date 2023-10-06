using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using TreinoSportAPI.MapperNoSQL.Connection;
using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.MapperNoSQL {
    public class TreinoMapperNoSQL {

        private readonly IMongoCollection<DiaDaSemanaDTO> dataHorarioDB;

        public TreinoMapperNoSQL() {}

        public TreinoMapperNoSQL(MongoDBConnection mongoDBConnection) {
            dataHorarioDB = mongoDBConnection.GetCollection<DiaDaSemanaDTO>("TreinoSport", "DataHorario");
        }

        public Task InserirHorarios(DiaDaSemanaDTO diaDaSemanaDTO) {
            CorrigirTimeZone(diaDaSemanaDTO);
            return dataHorarioDB.InsertOneAsync(diaDaSemanaDTO);
        }

        public async Task<List<DiaDaSemana>> BuscarHorarios(int codigoTreino) {
            var diasDaSemanaDTO = await dataHorarioDB.FindSync(dias => dias.CodigoTreino == codigoTreino).FirstOrDefaultAsync();
            if (diasDaSemanaDTO == null) {
                return new List<DiaDaSemana>();
            }
            var datasTreinos = diasDaSemanaDTO.DatasTreinos;
            return datasTreinos;
        }

        public async Task<DiaDaSemanaDTO> BuscarAlunosPresentes(int codigoTreino) {
            var treino = await dataHorarioDB.FindSync(dias => dias.CodigoTreino == codigoTreino).FirstOrDefaultAsync();
            return treino;
        }

        public async Task<List<DiaDaSemanaDTO>> BuscarTodosHorarios() {
            var filtro = Builders<DiaDaSemanaDTO>.Filter.Where(dto => dto.CodigoTreino > 0);
            var listaDto = (await dataHorarioDB.FindAsync(filtro)).ToList();
            return listaDto;
        }

        public async Task AtualizarDiasHorarios(DiaDaSemanaDTO diaDaSemanaDTO, bool naoCorrigir = false) {
            var filtro = Builders<DiaDaSemanaDTO>.Filter.Where(dto => dto.CodigoTreino == diaDaSemanaDTO.CodigoTreino);
            var update = Builders<DiaDaSemanaDTO>.Update.Set(dto => dto.DatasTreinos, diaDaSemanaDTO.DatasTreinos);
            if (!naoCorrigir) {
                CorrigirTimeZone(diaDaSemanaDTO);
            }

            await dataHorarioDB.UpdateOneAsync(filtro, update);
        }

        private void CorrigirTimeZone(DiaDaSemanaDTO dto) {
            foreach (var dia in dto.DatasTreinos) {
                var horariosCorrigidos = new List<Horario>();
                foreach (var horario in dia.Horarios) {

                    var horarioTemp = horario;

                    if (horario.Hora.Hour < 2) {
                        horarioTemp.Hora = horario.Hora.AddDays(1);
                    }

                    horarioTemp.Hora = horario.Hora.AddHours(-2);
                    horariosCorrigidos.Add(horarioTemp);
                }
                dia.Horarios = horariosCorrigidos;
            }
        }

    }
}
