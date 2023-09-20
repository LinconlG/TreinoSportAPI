namespace TreinoSportAPI.Models {
    public class ApiError {
        public string Message { get; set; }
        public bool IsPublicMessage { get; set; }

        public ApiError(string message, bool isPublicMessage) {
            Message = message;
            IsPublicMessage = isPublicMessage;
        }
    }
}
