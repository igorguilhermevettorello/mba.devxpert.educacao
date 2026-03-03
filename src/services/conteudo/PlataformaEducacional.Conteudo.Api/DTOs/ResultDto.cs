namespace PlataformaEducacional.Conteudo.Api.DTOs
{
    public class ResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }

        public ResultDto()
        {
            Errors = new List<string>();
        }

        public ResultDto(bool success, string message)
        {
            Success = success;
            Message = message;
            Errors = new List<string>();
        }

        public static ResultDto Ok(string message = "Operação realizada com sucesso")
        {
            return new ResultDto(true, message);
        }

        public static ResultDto Fail(string message, List<string> errors = null)
        {
            return new ResultDto(false, message)
            {
                Errors = errors ?? new List<string>()
            };
        }

        public static ResultDto<T> Ok<T>(T data, string message = "Operação realizada com sucesso")
        {
            return new ResultDto<T>(true, message, data);
        }

        public static ResultDto<T> Fail<T>(string message, List<string> errors = null)
        {
            return new ResultDto<T>(false, message, default)
            {
                Errors = errors ?? new List<string>()
            };
        }
    }

    public class ResultDto<T> : ResultDto
    {
        public T Data { get; set; }

        public ResultDto() : base()
        {
        }

        public ResultDto(bool success, string message, T data) : base(success, message)
        {
            Data = data;
        }
    }
}
