using Microsoft.AspNetCore.Mvc;
using System.Net.NetworkInformation;

namespace TreinoSportAPI.Utilities {
    public static class Extensions {

        public static int ObterCodigoConta(this ControllerBase controller) {

            // Obtém o código da conta do token
            var codigoConta = controller.User.FindFirst("CodigoConta")?.Value;

            if (string.IsNullOrEmpty(codigoConta)) {
                throw new APIException("Código da conta não encontrado no token.", true);
            }

            // Converte para int (ajuste conforme seu tipo real)
            if (!int.TryParse(codigoConta, out int codigo)) {
                throw new APIException("Código da conta inválido.", true);
            }

            return codigo;
        }
    }
}
