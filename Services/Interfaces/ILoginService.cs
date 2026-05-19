using TreinoSportAPI.Models;

namespace TreinoSportAPI.Services.Interfaces {
    public interface ILoginService {
        Task<Conta> Login(string email, string senha);
    }
}
