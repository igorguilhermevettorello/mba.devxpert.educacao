namespace PlataformaEducacional.Pagamentos.Api.Models
{
    public class CartaoCredito
    {
        //TODO: transformar em value object

        public string Titular { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string MesAnoVencimento { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;

        protected CartaoCredito() { }

        public CartaoCredito(string titular, string numero, string mesAnoVencimento, string cvv)
        {
            Titular = titular;
            Numero = numero;
            MesAnoVencimento = mesAnoVencimento;
            CVV = cvv;
        }
    }
}
