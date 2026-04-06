using AutoMapper;
using PlataformaEducacional.Conteudo.Api.DTOs.ConteudoProgramatico;
using PlataformaEducacional.Conteudo.Domain.ValueObjects;

namespace PlataformaEducacional.Conteudo.Api.AutoMapper
{
    public class ConteudoProgramaticoMappingProfile : Profile
    {
        public ConteudoProgramaticoMappingProfile()
        {
            CreateMap<ConteudoProgramatico, ConteudoProgramaticoDto>();
        }
    }
}
