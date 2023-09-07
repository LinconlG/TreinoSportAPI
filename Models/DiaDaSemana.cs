using MongoDB.Bson.Serialization.Attributes;

namespace TreinoSportAPI.Models {
    public class DiaDaSemana {
        public DayOfWeek Dia { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public List<DateTime> Horarios { get; set; }
    }
}
