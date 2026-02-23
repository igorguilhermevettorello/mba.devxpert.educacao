namespace PlataformaEducacional.Pagamentos.EducaPag
{
    public class EducaPagService
    {
        public readonly string ApiKey;
        public readonly string EncryptionKey;

        public EducaPagService(string apiKey, string encryptionKey)
        {
            ApiKey = apiKey;
            EncryptionKey = encryptionKey;
        }
    }
}
