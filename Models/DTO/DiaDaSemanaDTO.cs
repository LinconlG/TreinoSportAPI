using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace TreinoSportAPI.Models.DTO {
    public class DiaDaSemanaDTO {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public int CodigoTreino { get; set; }
        public List<DiaDaSemana> DatasTreinos { get; set; }
    }
}
