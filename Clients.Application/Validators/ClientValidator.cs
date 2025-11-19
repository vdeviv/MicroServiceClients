using FluentResults;
using Clients.Domain.Entities;
using Clients.Domain.Interfaces;
using System.Text.RegularExpressions;

namespace Clients.Application.Validators
{
    public class ClientValidator : IValidator<Client>
    {
        private static readonly Regex LettersAndSpaces =
            new(@"^[A-Za-zÁÉÍÓÚÜÑáéíóúüñ ]+$", RegexOptions.Compiled);

        private static readonly Regex Email =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        private static readonly Regex Nit =
            new(@"^[0-9]{7,12}(-[0-9]{1})?$", RegexOptions.Compiled);

        public Result Validate(Client c)
        {
            var r = Result.Ok();

            // =========================
            // NOMBRE
            // =========================
            if (string.IsNullOrWhiteSpace(c.first_name))
            {
                r = r.WithFieldError("first_name", "El nombre es obligatorio.");
            }
            else
            {
                var v = c.first_name.Trim();

                if (v.Length is < 2 or > 50)
                    r = r.WithFieldError("first_name", "El nombre debe tener entre 2 y 50 caracteres.");

                if (!LettersAndSpaces.IsMatch(v))
                    r = r.WithFieldError("first_name", "El nombre solo debe tener letras y espacios.");
            }

            // =========================
            // APELLIDO
            // =========================
            if (string.IsNullOrWhiteSpace(c.last_name))
            {
                r = r.WithFieldError("last_name", "El apellido es obligatorio.");
            }
            else
            {
                var v = c.last_name.Trim();

                if (v.Length is < 2 or > 50)
                    r = r.WithFieldError("last_name", "El apellido debe tener entre 2 y 50 caracteres.");

                if (!LettersAndSpaces.IsMatch(v))
                    r = r.WithFieldError("last_name", "El apellido solo debe contener letras y espacios.");
            }

            // =========================
            // EMAIL (opcional)
            // =========================
            if (!string.IsNullOrWhiteSpace(c.email))
            {
                var mail = c.email.Trim();

                if (mail.Length > 100)
                    r = r.WithFieldError("email", "El correo no debe exceder 100 caracteres.");

                if (!Email.IsMatch(mail))
                    r = r.WithFieldError("email", "El correo no tiene un formato válido.");
            }

            // =========================
            // NIT
            // =========================
            if (string.IsNullOrWhiteSpace(c.nit))
            {
                r = r.WithFieldError("nit", "El NIT es obligatorio.");
            }
            else
            {
                var v = c.nit.Trim();

                if (!Nit.IsMatch(v))
                    r = r.WithFieldError(
                        "nit",
                        "El NIT debe tener 7–12 dígitos, sin letras ni caracteres especiales (opcionalmente con guion y dígito verificador)."
                    );
            }

            return r;
        }
    }
}
