namespace TreinoSportAPI.Utilities {
    public class APIException : Exception {
        public string message { get; set; }
        public bool IsPublicMessage { get; set; }

        public APIException(string message, bool isPublicMessage) : base(message) {
            IsPublicMessage = isPublicMessage;
            this.message = message;
        }
    }
}
