using TreinoSportAPI.Models;
using TreinoSportAPI.Models.DTO;

namespace TreinoSportAPI.Mappers.Interfaces {
    public interface ILoginMapper {
        Task<Conta> CheckLogin(string email);
    }
}
