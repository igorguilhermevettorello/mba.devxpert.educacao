using AutoMapper;
using PlataformaEducacional.Conteudo.Api.DTOs.Aulas;
using PlataformaEducacional.Conteudo.Api.DTOs.ConteudoProgramatico;
using PlataformaEducacional.Conteudo.Api.DTOs.Cursos;
using PlataformaEducacional.Conteudo.Domain.Entities;
using PlataformaEducacional.Conteudo.Domain.ValueObjects;

namespace PlataformaEducacional.Conteudo.Api.Configuration
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // Mapeamento de Curso
            CreateMap<Curso, CursoDto>()
                .ForMember(dest => dest.ConteudoProgramatico, opt => opt.MapFrom(src => src.ConteudoProgramatico));

            CreateMap<ConteudoProgramatico, ConteudoProgramaticoDto>();

            // Mapeamento de Aula
            CreateMap<Aula, AulaDto>();
        }
    }
}