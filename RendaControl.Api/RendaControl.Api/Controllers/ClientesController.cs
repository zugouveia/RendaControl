using Microsoft.AspNetCore.Mvc;
using RendaControl.Api.Models;

namespace RendaControl.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private static List<Cliente> _clientes = new List<Cliente>();

        // GET
        [HttpGet]
        public IEnumerable<Cliente> Get()
        {
            return _clientes;
        }

        // POST
        [HttpPost]
        public IActionResult Post([FromBody] Cliente novoCliente)
        {
            if (novoCliente == null) return BadRequest();

            novoCliente.Id = _clientes.Count + 1;
            _clientes.Add(novoCliente);

            return CreatedAtAction(nameof(Get), new { id = novoCliente.Id }, novoCliente);
        }
    }
}