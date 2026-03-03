using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacional.Conteudo.Api.DTOs.Cursos
{
    public class ConteudoProgramaticoDto
    {
        [Required(ErrorMessage = "A ementa é obrigatória")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "A ementa deve ter entre 10 e 2000 caracteres")]
        public string Ementa { get; set; }

        [Required(ErrorMessage = "O objetivo é obrigatório")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "O objetivo deve ter entre 10 e 1000 caracteres")]
        public string Objetivo { get; set; }

        [Required(ErrorMessage = "A bibliografia é obrigatória")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "A bibliografia deve ter entre 10 e 2000 caracteres")]
        public string Bibliografia { get; set; }

        [Required(ErrorMessage = "A URL do material é obrigatória")]
        [StringLength(500, ErrorMessage = "A URL do material deve ter no máximo 500 caracteres")]
        [Url(ErrorMessage = "A URL do material não é válida")]
        public string MaterialUrl { get; set; }
    }
}
