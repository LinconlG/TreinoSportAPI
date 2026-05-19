using Microsoft.AspNetCore.Routing.Tree;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using TreinoSportAPI.Mappers.NoSQL.Connection;
using TreinoSportAPI.Mappers.Interfaces;
using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Mappers.NoSQL {
    public class TreinoMapperNoSQL : ITreinoMapperNoSQL {

        private readonly IMongoCollection<DiaDaSemanaDTO> dataHorarioDB;

        public TreinoMapperNoSQL(MongoDBConnection mongoDBConnection) {
            dataHorarioDB = mongoDBConnection.GetCollection<DiaDaSemanaDTO>("TreinoSport", "DataHorario");
        }

        /// <summary>
        /// Insere os horários de um treino no MongoDB.
        /// </summary>
        public Task InserirHorarios(DiaDaSemanaDTO diaDaSemanaDTO) {
            return dataHorarioDB.InsertOneAsync(diaDaSemanaDTO);
        }

        /// <summary>
        /// Busca os dias e horários de um treino pelo código do treino.
        /// </summary>
        public async Task<List<DiaDaSemana>> BuscarHorarios(int codigoTreino) {
            var diasDaSemanaDTO = await dataHorarioDB.FindSync(dias => dias.CodigoTreino == codigoTreino).FirstOrDefaultAsync();
            if (diasDaSemanaDTO == null) {
                return new List<DiaDaSemana>();
            }
            var datasTreinos = diasDaSemanaDTO.DatasTreinos;
            return datasTreinos;
        }

        /// <summary>
        /// Busca o documento completo de um treino incluindo alunos presentes por código do treino.
        /// </summary>
        public async Task<DiaDaSemanaDTO> BuscarAlunosPresentes(int codigoTreino) {
            var treino = await dataHorarioDB.FindSync(dias => dias.CodigoTreino == codigoTreino).FirstOrDefaultAsync();
            return treino;
        }

        /// <summary>
        /// Busca todos os documentos de horários de treinos cadastrados no MongoDB.
        /// </summary>
        public async Task<List<DiaDaSemanaDTO>> BuscarTodosHorarios() {
            var filtro = Builders<DiaDaSemanaDTO>.Filter.Where(dto => dto.CodigoTreino > 0);

            var listaDto = await dataHorarioDB.FindAsync(filtro);
            if (listaDto == null) {
                return new();
            }
            return listaDto.ToList();
        }

        /// <summary>
        /// Atualiza os dias e horários de um treino existente no MongoDB.
        /// </summary>
        public async Task AtualizarDiasHorarios(DiaDaSemanaDTO diaDaSemanaDTO) {
            var filtro = Builders<DiaDaSemanaDTO>.Filter.Where(dto => dto.CodigoTreino == diaDaSemanaDTO.CodigoTreino);
            var update = Builders<DiaDaSemanaDTO>.Update.Set(dto => dto.DatasTreinos, diaDaSemanaDTO.DatasTreinos);

            await dataHorarioDB.UpdateOneAsync(filtro, update);
        }

        /// <summary>
        /// Deleta o documento de horários de um treino pelo código do treino.
        /// </summary>
        public Task DeletarHorarios(int codigoTreino) {
            var filtro = Builders<DiaDaSemanaDTO>.Filter.Where(dto => dto.CodigoTreino == codigoTreino);
            return dataHorarioDB.DeleteOneAsync(filtro);
        }

    }
}
