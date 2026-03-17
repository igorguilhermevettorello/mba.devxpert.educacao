using AutoMapper;
using PlataformaEducacional.Conteudo.Api.DTOs.Aulas;
using PlataformaEducacional.Conteudo.Domain.Entities;

namespace PlataformaEducacional.Conteudo.Api.AutoMapper
{
    public class AulaMappingProfile : Profile
    {
        public AulaMappingProfile()
        {
            CreateMap<Aula, AulaDto>();
        }
    }
}
