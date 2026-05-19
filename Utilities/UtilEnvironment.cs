namespace TreinoSportAPI.Utilities {
    public static class UtilEnvironment {

        public static double ConverteHorasToMs(int horas) {
            var tempSpan = new TimeSpan(horas, 0, 0);       
            return tempSpan.TotalMilliseconds;
        }

        public static string GerarToken() {
            var guid = Guid.NewGuid().ToString();
            guid = guid.Replace("-", "");
            var token = "";

            Random random = new Random();

            for (int i = 0; i < 4; i++) {
                var indexAleatorio = random.Next(guid.Length - 1);
                token += guid[indexAleatorio];
            }
            token = token.ToUpper();
            return token;
        }
    }
}
