using System.ComponentModel.DataAnnotations;

namespace TreinoSportAPI.Models {
    public class Conta {
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O email informado não é válido.")]
        [MaxLength(150, ErrorMessage = "O email pode ter no máximo 150 caracteres.")]
        public string Email { get; set; }

        [MaxLength(255, ErrorMessage = "A senha pode ter no máximo 255 caracteres.")]
        public string Senha { get; set; }

        [MaxLength(500, ErrorMessage = "A descrição pode ter no máximo 500 caracteres.")]
        public string Descricao { get; set; }

        public bool IsCentroTreinamento { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [MaxLength(9, ErrorMessage = "O CEP deve ter no máximo 9 caracteres.")]
        public string? Cep { get; set; }
    }
}
