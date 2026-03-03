using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacional.Alunos.Api.Models
{
    public class MatricularAlunoViewModel
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid AlunoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid CursoId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public decimal Valor { get; set; }
    }
}
