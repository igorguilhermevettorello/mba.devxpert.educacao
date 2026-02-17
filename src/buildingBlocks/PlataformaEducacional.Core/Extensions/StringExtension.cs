namespace PlataformaEducacional.Core.Extensions;

public static class StringExtension
{
    public static string ApenasNumeros(this string str, string input)
    {
        return new string(input.Where(char.IsDigit).ToArray());
    }
}
