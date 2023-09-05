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

        public Task InserirHorarios(DiaDaSemanaDTO dataHorarios) {
            return dataHorarioDB.InsertOneAsync(dataHorarios);
        }

        public async Task<List<DiaDaSemana>> BuscarHorarios(int codigoTreino) {
            var diasDaSemanaDTO = await dataHorarioDB.FindSync(dias => dias.CodigoTreino == codigoTreino).FirstOrDefaultAsync();
            var datasTreinos = diasDaSemanaDTO.DatasTreinos;
            return datasTreinos;
        }

        public async Task AtualizarHorarios(DiaDaSemanaDTO diaDaSemanaDTO) {
            var filtro = Builders<DiaDaSemanaDTO>.Filter.Where(dto => dto.CodigoTreino == diaDaSemanaDTO.CodigoTreino);
            var update = Builders<DiaDaSemanaDTO>.Update.Set(dto => dto.DatasTreinos, diaDaSemanaDTO.DatasTreinos);
            await dataHorarioDB.UpdateOneAsync(filtro, update);
        }
    }
}
