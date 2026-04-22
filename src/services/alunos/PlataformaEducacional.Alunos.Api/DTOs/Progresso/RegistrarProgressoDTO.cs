using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacional.Alunos.Api.DTOs.Progresso
{
    public class RegistrarProgressoDTO
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid AulaId { get; set; }
    }
}
