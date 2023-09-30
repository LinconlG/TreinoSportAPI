namespace TreinoSportAPI.Services.Interfaces {
    public interface IEmailService {

        Task SendPasswordCode(string email, string token);
    }
}
