using System.ComponentModel.DataAnnotations;

namespace PlataformaEducacional.Alunos.Api.DTOs.Enderecos
{
    public class AdicionarEnderecoDTO
    {
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Logradouro { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Numero { get; set; }

        public string Complemento { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Cep { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string Estado { get; set; }
    }
}
