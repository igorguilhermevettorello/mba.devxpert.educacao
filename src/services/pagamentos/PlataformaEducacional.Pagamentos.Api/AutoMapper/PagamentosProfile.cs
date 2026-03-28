using AutoMapper;
using PlataformaEducacional.Pagamentos.Api.Models;
using PlataformaEducacional.Pagamentos.Api.Models.DTOs;

namespace PlataformaEducacional.Pagamentos.Api.AutoMapper
{
    public class PagamentosProfile : Profile
    {
        public PagamentosProfile()
        {
            CreateMap<Pagamento, PagamentoDto>();
            CreateMap<Transacao, TransacaoDto>();
        }
    }
}
