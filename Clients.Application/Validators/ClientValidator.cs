
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

            if (string.IsNullOrWhiteSpace(c.first_name))
                r = r.WithError(new Error("El nombre es obligatorio.").WithMetadata("FieldName", "first_name"));
            else
            {
                var v = c.first_name.Trim();
                if (v.Length is < 2 or > 50) r = r.WithError(new Error("Debe tener entre 2 y 50 caracteres.").WithMetadata("FieldName", "first_name"));
                if (!LettersAndSpaces.IsMatch(v)) r = r.WithError(new Error("El nombre solo debe tener letras y espacios.").WithMetadata("FieldName", "first_name"));
            }

            if (string.IsNullOrWhiteSpace(c.last_name))
                r = r.WithError(new Error("El apellido es obligatorio.").WithMetadata("FieldName", "last_name"));
            else
            {
                var v = c.last_name.Trim();
                if (v.Length is < 2 or > 50) r = r.WithError(new Error("Debe tener entre 2 y 50 caracteres.").WithMetadata("FieldName", "last_name"));
                if (!LettersAndSpaces.IsMatch(v)) r = r.WithError(new Error("El apellido solo debe letras y espacios.").WithMetadata("FieldName", "last_name"));
            }

            if (!string.IsNullOrWhiteSpace(c.email))
            {
                var mail = c.email.Trim();
                if (mail.Length > 100) r = r.WithError(new Error("No debe exceder 100 caracteres.").WithMetadata("FieldName", "email"));
                if (!Email.IsMatch(mail)) r = r.WithError(new Error("Formato de correo inválido.").WithMetadata("FieldName", "email"));
            }

            if (string.IsNullOrWhiteSpace(c.nit))
                r = r.WithError(new Error("El NIT es obligatorio.").WithMetadata("FieldName", "nit"));
            else
            {
                var v = c.nit.Trim();
                if (!Nit.IsMatch(v)) r = r.WithError(new Error("El NIT debe tener 7–12 dígitos sin letras ni caracteres especiales.").WithMetadata("FieldName", "nit"));
            }

            return r;
        }
    }
}