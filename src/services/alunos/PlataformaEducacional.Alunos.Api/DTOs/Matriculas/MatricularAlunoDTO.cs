using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacional.Alunos.Api.DTOs.Matriculas
{
    public class MatricularAlunoDTO
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid CursoId { get; set; }
    }
}