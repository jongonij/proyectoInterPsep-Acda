using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GestorPedidosRestaurante_API.Models;
using GestorPedidosRestaurante_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GestorPedidosRestaurante.Sockets;

namespace GestorPedidosRestaurante_API.Controllers
{
    [ApiController]
    [Route("api/pedidos")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;
        private readonly ISocketService _socketService;
        private readonly JsonFileService _jsonFileService;
        private readonly SocketServer _socketServer;
        private List<Pedido> _pedidos;
        private List<Administrador> _administradores; // Agregamos la lista de administradores

        public PedidoController(IPedidoService pedidoService, ISocketService socketService, JsonFileService jsonFileService, SocketServer socketServer)
        {
            _pedidoService = pedidoService;
            _socketService = socketService;
            _jsonFileService = jsonFileService;
            _socketServer = socketServer;

            // Cargar datos desde JSON
            _pedidos = _jsonFileService.CargarPedidosAsync().Result;
            _administradores = _jsonFileService.CargarAdminsAsync().Result; // Cargar administradores
        }

        [HttpPost]
        public async Task<IActionResult> CrearPedido([FromBody] Pedido pedido)
        {
            pedido.Id = Guid.NewGuid();
            _pedidos.Add(pedido);
            await _jsonFileService.GuardarPedidosAsync(_pedidos); // Guardar en JSON

            // Notificar a los clientes via sockets
            await _socketService.NotificarCambioEstado(pedido);

            return Ok(pedido);
        }


          [HttpPut("actualizar-estado/{id}")]
        public async Task<IActionResult> ActualizarEstadoPedido(Guid id, [FromBody] ActualizarEstadoRequest request)
        {
            var pedido = _pedidos.Find(p => p.Id == id);
            if (pedido == null)
            {
                return NotFound($"❌ Pedido con ID {id} no encontrado.");
            }

            pedido.Estado = request.NuevoEstado;
            await _jsonFileService.GuardarPedidosAsync(_pedidos); // Guardar cambios en JSON

            // Notificar cambio de estado a través del servidor de sockets
            await _socketServer.NotificarClientes($"El estado del pedido {pedido.Id} ha sido actualizado a {pedido.Estado}");

            return Ok("✅ Estado del pedido actualizado con éxito.");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPedido(Guid id)
        {
            var pedido = _pedidos.Find(p => p.Id == id);
            if (pedido == null)
            {
                return NotFound($"❌ Pedido con ID {id} no encontrado.");
            }
            await _socketServer.NotificarClientes($"El pedido {pedido.Id} ha sido obtenido");
            return Ok(pedido);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerPedidos()
        {
            return Ok(_pedidos);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPedido(Guid id)
        {
            var pedido = _pedidos.Find(p => p.Id == id);
            if (pedido == null)
            {
                return NotFound($"❌ Pedido con ID {id} no encontrado.");
            }

            _pedidos.Remove(pedido);
            await _jsonFileService.GuardarPedidosAsync(_pedidos); // Guardar en JSON

            return NoContent(); // Pedido eliminado
        }
    }
}
