using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacional.Bff.Api.Models;

public class RealizarMatriculaDto
{
    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid AlunoId { get; set; }

    [Required(ErrorMessage = "O campo {0} é obrigatório")]
    public Guid CursoId { get; set; }
}
