
using FluentResults;

namespace Clients.Domain.Interfaces
{
    public interface IValidator<T>
    {
        Result Validate(T entity);
    }
}