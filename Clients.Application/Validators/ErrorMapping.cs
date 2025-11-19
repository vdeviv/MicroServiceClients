using FluentResults;

namespace Clients.Application.Validators
{
    /// <summary>
    /// Convierte la lista de errores de FluentResults en
    /// un diccionario { campo : "mensaje1; mensaje2; ..." }.
    /// </summary>
    public static class ErrorMapping
    {
        public static Dictionary<string, string> ToDictionary(this IReadOnlyList<IError> errors)
        {
            return errors
                .GroupBy(e =>
                {
                    if (e.Metadata.TryGetValue("field", out var f) && f is not null)
                        return f.ToString() ?? string.Empty;

                    return string.Empty; // errores sin campo → clave vacía
                })
                .ToDictionary(
                    g => g.Key,
                    g => string.Join("; ", g.Select(e => e.Message))
                );
        }
    }
}
