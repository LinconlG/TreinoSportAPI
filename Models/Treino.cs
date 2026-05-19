using System.ComponentModel.DataAnnotations;
using TreinoSportAPI.Models.Enums;

namespace TreinoSportAPI.Models {
    public class Treino {
        public int Codigo { get; set; }

        [Required(ErrorMessage = "O nome do treino é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome pode ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [MaxLength(500, ErrorMessage = "A descrição pode ter no máximo 500 caracteres.")]
        public string Descricao { get; set; }

        public List<Conta> Alunos { get; set; }
        public DateTime DataCriacao { get; set; }
        public Conta Criador { get; set; }
        public List<DiaDaSemana> DatasTreinos { get; set; }
        public DateTime DataVencimento { get; set; }
        public ModalidadeTreino Modalidade { get; set; }

        [Range(1, 1000, ErrorMessage = "O limite de alunos deve ser entre 1 e 1000.")]
        public int LimiteAlunos { get; set; }
    }
}
