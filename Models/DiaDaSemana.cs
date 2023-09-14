using MongoDB.Bson.Serialization.Attributes;

namespace TreinoSportAPI.Models {
    public class DiaDaSemana {
        public DayOfWeek Dia { get; set; }
        public List<Horario> Horarios { get; set; }
    }
}
