using Clients.Application.Interfaces;
using Clients.Application.Validators;
using Clients.Domain.Entities;
using Clients.Domain.Interfaces;
using FluentResults;
using System.Text.RegularExpressions;

namespace Clients.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly IRepository<Client> _clientRepository;
        private readonly IValidator<Client> _validator;

        public ClientService(IRepository<Client> clientRepository, IValidator<Client> validator)
        {
            _clientRepository = clientRepository;
            _validator = validator;
        }

        private static readonly Regex MultiSpace = new(@"\s+", RegexOptions.Compiled);

        private static string NormalizeName(string? s)
            => string.IsNullOrWhiteSpace(s) ? string.Empty : MultiSpace.Replace(s.Trim(), " ");

        private static string NormalizeEmail(string? s)
            => string.IsNullOrWhiteSpace(s) ? string.Empty : s.Trim().ToLowerInvariant();

        public async Task<Client> RegisterAsync(Client entity, int actorId)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            entity.first_name = NormalizeName(entity.first_name);
            entity.last_name = NormalizeName(entity.last_name);
            entity.email = NormalizeEmail(entity.email);
            entity.nit = entity.nit?.Trim() ?? string.Empty;

            var validationResult = _validator.Validate(entity);
            if (validationResult.IsFailed)
                throw new ValidationException(
                    "Validación de dominio falló para Cliente.",
                    validationResult.Errors.ToDictionary()
                );

            var all = await _clientRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(entity.email) &&
                all.Any(c => !c.is_deleted &&
                    string.Equals((c.email ?? "").Trim(), entity.email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DomainException("El correo ya existe.");
            }

            if (!string.IsNullOrWhiteSpace(entity.nit) &&
                all.Any(c => !c.is_deleted &&
                    string.Equals((c.nit ?? "").Trim(), entity.nit, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DomainException("El NIT ya existe.");
            }

            var now = DateTime.Now;

            entity.is_deleted = false;
            entity.created_by = actorId;
            entity.created_at = now;
            entity.updated_by = actorId;
            entity.updated_at = now;

            return await _clientRepository.Create(entity);
        }

        public async Task<Client?> GetByIdAsync(int id)
        {
            return await _clientRepository.GetById(new Client { id = id });
        }

        public async Task<IEnumerable<Client>> ListAsync()
        {
            return await _clientRepository.GetAll();
        }

        public async Task UpdateAsync(Client entity, int actorId)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            var current = await _clientRepository.GetById(new Client { id = entity.id })
                          ?? throw new NotFoundException($"Cliente con ID {entity.id} no encontrado.");

            current.first_name = NormalizeName(entity.first_name);
            current.last_name = NormalizeName(entity.last_name);
            current.email = NormalizeEmail(entity.email);
            current.nit = entity.nit?.Trim() ?? string.Empty;

            var validationResult = _validator.Validate(current);
            if (validationResult.IsFailed)
                throw new ValidationException(
                    "Validación de dominio falló para Cliente.",
                    validationResult.Errors.ToDictionary()
                );

            var all = await _clientRepository.GetAll();

            if (!string.IsNullOrWhiteSpace(current.email) &&
                all.Any(c => c.id != current.id && !c.is_deleted &&
                             string.Equals((c.email ?? "").Trim(), current.email, StringComparison.OrdinalIgnoreCase)))
                throw new DomainException("El correo ya existe.");

            if (!string.IsNullOrWhiteSpace(current.nit) &&
                all.Any(c => c.id != current.id && !c.is_deleted &&
                             string.Equals((c.nit ?? "").Trim(), current.nit, StringComparison.OrdinalIgnoreCase)))
                throw new DomainException("El NIT ya existe.");

            current.updated_by = actorId;
            current.updated_at = DateTime.Now;

            await _clientRepository.Update(current);
        }

        public async Task SoftDeleteAsync(int id, int actorId)
        {
            var current = await _clientRepository.GetById(new Client { id = id })
                          ?? throw new NotFoundException($"Cliente con ID {id} no encontrado.");

            current.is_deleted = true;
            current.updated_by = actorId;
            current.updated_at = DateTime.Now;

            await _clientRepository.Delete(current);
        }
    }

    public class DomainException : Exception { public DomainException(string m) : base(m) { } }

    public class NotFoundException : Exception { public NotFoundException(string m) : base(m) { } }

    public class ValidationException : Exception
    {
        public Dictionary<string, string> Errors { get; }

        public ValidationException(string message, Dictionary<string, string> errors)
            : base(message)
        {
            Errors = errors;
        }
    }
}
