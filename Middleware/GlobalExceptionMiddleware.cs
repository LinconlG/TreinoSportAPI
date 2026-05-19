using System.Net;
using System.Text.Json;
using TreinoSportAPI.Models;
using TreinoSportAPI.Utilities;

namespace TreinoSportAPI.Middleware {
    public class GlobalExceptionMiddleware : IMiddleware {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next) {
            try {
                await next(context);
            }
            catch (APIException ex) {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                var error = new ApiError(ex.Message, true);
                await context.Response.WriteAsync(JsonSerializer.Serialize(error));
            }
            catch (Exception ex) {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                var error = new ApiError("Ocorreu um erro interno no servidor.", false);
                await context.Response.WriteAsync(JsonSerializer.Serialize(error));
            }
        }
    }
}
