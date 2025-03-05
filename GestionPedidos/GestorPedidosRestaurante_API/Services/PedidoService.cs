using System;
using System.Threading.Tasks;
using GestorPedidosRestaurante_API.Models;

namespace GestorPedidosRestaurante_API.Services
{
    public interface IPedidoService
    {
        Task<Pedido> CrearPedido(Pedido pedido);
        Task<Pedido> ActualizarEstado(Guid id, EstadoPedido nuevoEstado);
        Task<Pedido> ObtenerPedido(Guid id);
        Task<bool> EliminarPedido(Guid id);
        Task<IEnumerable<Pedido>> ObtenerPedidos();  // Usamos IEnumerable en vez de List
    }


    public class PedidoService : IPedidoService
    {
        private static List<Pedido> _pedidos = new List<Pedido>();  // Lista simulada de pedidos

        public Task<Pedido> CrearPedido(Pedido pedido)
        {
            pedido.Id = Guid.NewGuid();
            _pedidos.Add(pedido);
            return Task.FromResult(pedido);
        }

        public Task<Pedido> ActualizarEstado(Guid id, EstadoPedido nuevoEstado)
        {
            var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido != null)
            {
                pedido.Estado = nuevoEstado;
            }
            return Task.FromResult(pedido);
        }


        public Task<Pedido> ObtenerPedido(Guid id)
        {
            var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(pedido);
        }

        public Task<bool> EliminarPedido(Guid id)
        {
            var pedido = _pedidos.FirstOrDefault(p => p.Id == id);
            if (pedido != null)
            {
                _pedidos.Remove(pedido);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<IEnumerable<Pedido>> ObtenerPedidos()
        {
            return Task.FromResult((IEnumerable<Pedido>)_pedidos);  // Cambiado a IEnumerable
        }
    }

}
