using TreinoSportAPI.Models;

namespace TreinoSportAPI.Services.Interfaces {
    public interface IAuthService {
        Task<Conta?> Authenticate(Conta user);
        string GenerateToken(Conta user);
    }
}
