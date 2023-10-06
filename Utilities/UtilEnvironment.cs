using Microsoft.AspNetCore.Mvc;
using System.Net;
using TreinoSportAPI.Models;

namespace TreinoSportAPI.Utilities {
    public static class UtilEnvironment {

        public static string ConnectionString;

        public static void Load(IConfiguration config) {
            ConnectionString = config.GetConnectionString("DataBaseConnection");
        }

        public static ObjectResult InternalServerError(this ControllerBase controller, ApiError apiError) {
            controller.Response.ContentType = "application/json";
            return controller.StatusCode((int)HttpStatusCode.InternalServerError, apiError);
        }

        public static ObjectResult InternalServerError(this ControllerBase controller, string message, bool isPublicMessage) {
            return InternalServerError(controller, new ApiError(message, isPublicMessage));
        }

        public static bool IsPublicMessageCheck(this Exception exception) {
            try {
                return ((APIException)exception).IsPublicMessage;
            }
            catch (Exception) {
                return false;
            }
        }

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
