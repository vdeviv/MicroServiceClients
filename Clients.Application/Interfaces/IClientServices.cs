
using Clients.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Clients.Application.Interfaces
{
    public interface IClientService
    {
        Task<Client> RegisterAsync(Client entity, int actorId);
        Task<Client?> GetByIdAsync(int id);
        Task<IEnumerable<Client>> ListAsync();
        Task UpdateAsync(Client entity, int actorId);
        Task SoftDeleteAsync(int id, int actorId);
    }
}