using GestorPedidosRestaurante.Sockets;
using GestorPedidosRestaurante_API.Models;

namespace GestorPedidosRestaurante_API.Services
{
    public interface ISocketService
    {
        Task NotificarCambioEstado(Pedido pedido);
    }

    public class SocketService : ISocketService
    {
        private readonly SocketServer _socketServer;

        public SocketService(SocketServer socketServer)
        {
            _socketServer = socketServer;
        }

        public async Task NotificarCambioEstado(Pedido pedido)
        {
            var mensaje = $"Pedido {pedido.Id} actualizado a {pedido.Estado}";
            await _socketServer.NotificarClientes(mensaje);
        }
    }
}