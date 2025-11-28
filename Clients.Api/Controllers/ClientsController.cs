using Clients.Application.Interfaces;
using Clients.Application.Services;
using Clients.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Clients.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;

        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }

        private static int ParseActorId(string? header)
        {
            return int.TryParse(header, out var id) ? id : 0;
        }

        // POST api/clients
        [HttpPost]
        [ProducesResponseType(typeof(Client), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Register(
            [FromBody] Client client,
            [FromHeader(Name = "X-Actor-Id")] string? actorHeader)
        {
            try
            {
                var actorId = ParseActorId(actorHeader);
                var createdClient = await _clientService.RegisterAsync(client, actorId);
                return CreatedAtAction(nameof(GetById), new { id = createdClient.id }, createdClient);
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
        }

        // PUT api/clients/{id}
        [HttpPut("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] Client client,
            [FromHeader(Name = "X-Actor-Id")] string? actorHeader)
        {
            try
            {
                var actorId = ParseActorId(actorHeader);
                client.id = id;
                await _clientService.UpdateAsync(client, actorId);
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
                return NotFound(new { error = $"Cliente con ID {id} no encontrado." });
            }

            return Ok(client);
        }

        // DELETE api/clients/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(
            int id,
            [FromHeader(Name = "X-Actor-Id")] string? actorHeader)
        {
            try
            {
                var actorId = ParseActorId(actorHeader);
                await _clientService.SoftDeleteAsync(id, actorId);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
    }
}
