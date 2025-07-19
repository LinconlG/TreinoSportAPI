using System.Text.Json.Serialization;

namespace TreinoSportAPI.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ModalidadeTreino {
        BeachTennis,
        Danca,
        Funcional,
        Futebol,
        Futsal,
        Futevolei,
        Ginastica,
        JiuJitsu,
        MuaiThai,
        Natacao,
        Pilates,
        Tenis,
        Volei
    }
}
