using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacional.Alunos.Api.DTOs.Certificados
{
    public class EmitirCertificadoDTO
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid MatriculaId { get; set; }
    }
}
