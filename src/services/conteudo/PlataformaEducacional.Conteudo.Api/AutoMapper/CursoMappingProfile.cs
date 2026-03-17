using AutoMapper;
using PlataformaEducacional.Conteudo.Api.DTOs.Cursos;
using PlataformaEducacional.Conteudo.Domain.Entities;

namespace PlataformaEducacional.Conteudo.Api.AutoMapper
{
    public class CursoMappingProfile : Profile
    {
        public CursoMappingProfile()
        {
            CreateMap<Curso, CursoDto>();
        }
    }
}
