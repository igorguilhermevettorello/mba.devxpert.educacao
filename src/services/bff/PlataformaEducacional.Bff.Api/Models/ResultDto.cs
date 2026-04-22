namespace PlataformaEducacional.Bff.Api.Models;

public class ResultDto<T> : ResultDto
{
    public T Data { get; set; }
}
