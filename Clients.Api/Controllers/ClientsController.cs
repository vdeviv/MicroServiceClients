using Clients.Application.Interfaces;
using Clients.Application.Services;
using Clients.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Clients.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private const int ActorId = 1; // de momento fijo

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        // POST api/clients
        [HttpPost]
        [ProducesResponseType(typeof(Client), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register([FromBody] Client client)
        {
            try
            {
                var createdClient = await _clientService.RegisterAsync(client, ActorId);
                return CreatedAtAction(nameof(GetById), new { id = createdClient.id }, createdClient);
            }
            catch (ValidationException ex)
            {
                // ⬅ Igual que en UserController: message + errors
                return BadRequest(new
                {
                    message = ex.Message,
                    errors = ex.Errors
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // PUT api/clients/{id}
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] Client client)
        {
            try
            {
                client.id = id;
                await _clientService.UpdateAsync(client, ActorId);
                return NoContent();
            }
            catch (ValidationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message,
                    errors = ex.Errors
                });
            }
            catch (DomainException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        // GET api/clients
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Client>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetAll()
        {
            var clients = await _clientService.ListAsync();
            return Ok(clients);
        }

        // GET api/clients/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Client), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var client = await _clientService.GetByIdAsync(id);
            if (client == null)
            {
                // para ser consistente, devuelvo JSON
                return NotFound(new { error = $"Cliente con ID {id} no encontrado." });
            }

            return Ok(client);
        }

        // DELETE api/clients/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _clientService.SoftDeleteAsync(id, ActorId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
