using System.Text;

namespace PlataformaEducacional.Core.Extensions;

public static class ExceptionExtesions
{
    /// <summary>
    /// Retorna a mensagem completa da exceção, incluindo mensagens de exceções internas.
    /// </summary>
    /// <param name="exception">A exceção da qual obter a mensagem completa.</param>
    /// <returns>A mensagem completa da exceção.</returns>
    public static string GetFullMessage(this Exception exception)
    {
        var message = new StringBuilder();
        message.AppendLine(exception.Message);

        if (exception.InnerException != null)
        {
            message.AppendLine(exception.InnerException.GetFullMessage());
        }

        return message.ToString();
    }
}
