using System;
using System.Collections.Generic;

namespace GestorPedidosRestaurante_API.Models
{
    public class Pedido
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public List<ItemPedido> Items { get; set; }
        public EstadoPedido Estado { get; set; }
        public Pedido()
        {
            Items = new List<ItemPedido>();
            Estado = new EstadoPedido();
        }
    }

    public class ItemPedido
    {
        public Guid ProductoId { get; set; }
        public string Nombre { get; set; }

        public int Cantidad { get; set; }
    }
    public enum EstadoPedido
{
    Pendiente,
    EnProceso,
    Completado,
    Cancelado
}

}