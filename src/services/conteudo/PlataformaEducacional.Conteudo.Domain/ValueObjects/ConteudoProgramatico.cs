namespace PlataformaEducacional.Conteudo.Domain.ValueObjects
{
    public class ConteudoProgramatico
    {
        public string Ementa { get; private set; }
        public string Objetivo { get; private set; }
        public string Bibliografia { get; private set; }
        public string MaterialUrl { get; private set; }
        private ConteudoProgramatico() { }

        public ConteudoProgramatico(string ementa, string objetivo, string bibliografia, string materialUrl)
        {
            Ementa = ementa;
            Objetivo = objetivo;
            Bibliografia = bibliografia;
            MaterialUrl = materialUrl;

            ValidarConteudoProgramatico();
        }

        private void ValidarConteudoProgramatico()
        {
            ValidarEmenta(Ementa);
            ValidarObjetivo(Objetivo);
            ValidarBibliografia(Bibliografia);
            ValidarMaterialUrl(MaterialUrl);
        }

        public void AtualizarEmenta(string ementa)
        {
            ValidarEmenta(ementa);
            Ementa = ementa;
        }

        public void AtualizarObjetivo(string objetivo)
        {
            ValidarObjetivo(objetivo);
            Objetivo = objetivo;
        }

        public void AtualizarBibliografia(string bibliografia)
        {
            ValidarBibliografia(bibliografia);
            Bibliografia = bibliografia;
        }

        public void AtualizarMaterialUrl(string materialUrl)
        {
            ValidarMaterialUrl(materialUrl);
            MaterialUrl = materialUrl;
        }

        // Métodos de validação privados
        private static void ValidarEmenta(string ementa)
        {
            if (string.IsNullOrWhiteSpace(ementa))
                throw new ArgumentException("Ementa do conteúdo programático é obrigatória", nameof(ementa));

            if (ementa.Length < 10)
                throw new ArgumentException("Ementa deve ter no mínimo 10 caracteres", nameof(ementa));

            if (ementa.Length > 5000)
                throw new ArgumentException("Ementa deve ter no máximo 5000 caracteres", nameof(ementa));
        }

        private static void ValidarObjetivo(string objetivo)
        {
            if (string.IsNullOrWhiteSpace(objetivo))
                throw new ArgumentException("Objetivo do conteúdo programático é obrigatório", nameof(objetivo));

            if (objetivo.Length < 10)
                throw new ArgumentException("Objetivo deve ter no mínimo 10 caracteres", nameof(objetivo));

            if (objetivo.Length > 2000)
                throw new ArgumentException("Objetivo deve ter no máximo 2000 caracteres", nameof(objetivo));
        }

        private static void ValidarBibliografia(string bibliografia)
        {
            if (string.IsNullOrWhiteSpace(bibliografia))
                throw new ArgumentException("Bibliografia do conteúdo programático é obrigatória", nameof(bibliografia));

            if (bibliografia.Length < 10)
                throw new ArgumentException("Bibliografia deve ter no mínimo 10 caracteres", nameof(bibliografia));

            if (bibliografia.Length > 3000)
                throw new ArgumentException("Bibliografia deve ter no máximo 3000 caracteres", nameof(bibliografia));
        }

        private static void ValidarMaterialUrl(string materialUrl)
        {
            if (string.IsNullOrWhiteSpace(materialUrl))
                throw new ArgumentException("URL do material do conteúdo programático é obrigatória", nameof(materialUrl));

            if (materialUrl.Length > 500)
                throw new ArgumentException("URL do material deve ter no máximo 500 caracteres", nameof(materialUrl));

            // Validação básica de URL
            if (!Uri.TryCreate(materialUrl, UriKind.Absolute, out var uriResult) ||
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("URL do material inválida. Deve ser uma URL HTTP ou HTTPS válida", nameof(materialUrl));
            }
        }
    }
}
